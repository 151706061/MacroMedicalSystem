#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using Macro.Common.Utilities;
using Macro.Desktop;
using Macro.Dicom;
using Macro.Dicom.Utilities.Anonymization;
using Macro.Common;
using System.IO;
using Macro.ImageViewer.Common.Auditing;
using System.Threading;
using Path=System.IO.Path;

namespace Macro.ImageViewer.Utilities.StudyFilters.Export
{
	public class DicomFileExporter
	{
		private readonly ICollection<string> _files;
		private DicomAnonymizer _anonymizer;
		private volatile bool _overwrite;
		private volatile bool _canceled;
		private volatile AuditedInstances _exportedInstances;
		private SynchronizationContext _synchronizationContext;
		private IDesktopWindow _desktopWindow;

		public DicomFileExporter(ICollection<string> files)
		{
			Platform.CheckPositive(files.Count, "files.Count");
			_files = files;
		}

		public bool Anonymize { get; set; }
		public string OutputPath { get; set; }

		public IDesktopWindow DesktopWindow
		{
			get { return _desktopWindow ?? Application.ActiveDesktopWindow; }
			set { _desktopWindow = value; }
		}

		public bool Export()
		{
			if (!Initialize())
				return false;

			EventResult result = EventResult.Success;

			try
			{
				BackgroundTask task = new BackgroundTask(DoExport, true);
				ProgressDialog.Show(task, DesktopWindow, true, ProgressBarStyle.Continuous);
				
				if (_canceled)
					result = EventResult.MinorFailure;

				return !_canceled;
			}
			catch
			{
				result = EventResult.MinorFailure;
				throw;
			}
			finally
			{
				AuditHelper.LogExportStudies(_exportedInstances, EventSource.CurrentUser, result);
			}
		}

		private bool Initialize()
		{
			_synchronizationContext = SynchronizationContext.Current;

			_exportedInstances = new AuditedInstances();
			_canceled = false;
			_overwrite = false;

			if (Anonymize)
			{
				ExportComponent component = new ExportComponent();
				component.OutputPath = OutputPath;

				if (DialogBoxAction.Ok != DesktopWindow.ShowDialogBox(component, SR.Export))
					return false;

				OutputPath = component.OutputPath;

				StudyData studyData = new StudyData
				{
					PatientId = component.PatientId,
					PatientsNameRaw = component.PatientsName,
					PatientsBirthDate = component.PatientsDateOfBirth,
					StudyId = component.StudyId,
					StudyDescription = component.StudyDescription,
					AccessionNumber = component.AccessionNumber,
					StudyDate = component.StudyDate
				};

				_anonymizer = new DicomAnonymizer();
				_anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;
				_anonymizer.StudyDataPrototype = studyData;
			}
			else
			{
				SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
				args.Prompt = SR.MessageSelectOutputLocation;
				args.Path = OutputPath;

				FileDialogResult result = DesktopWindow.ShowSelectFolderDialogBox(args);
				if (result.Action != DialogBoxAction.Ok)
					return false;

				OutputPath = result.FileName;
			}

			return true;
		}

		private void SaveFile(string filename)
		{
			if (_anonymizer != null)
			{
				DicomFile dicomFile = new DicomFile(filename);
				dicomFile.Load(); 

				_anonymizer.Anonymize(dicomFile);

				//anonymize first, then audit, since this is what gets exported.
				_exportedInstances.AddInstance(
				dicomFile.DataSet[DicomTags.PatientId].ToString(),
				dicomFile.DataSet[DicomTags.PatientsName].ToString(),
				dicomFile.DataSet[DicomTags.StudyInstanceUid].ToString(),
				filename);
				
				string fileName = System.IO.Path.Combine(OutputPath, dicomFile.MediaStorageSopInstanceUid);
				fileName += ".dcm";
				CheckFileExists(fileName); // this will never happen for anonymized images.
				if (_canceled)
					return;

				dicomFile.Save(fileName);
			}
			else
			{
				_exportedInstances.AddPath(filename, false);

				string destination = Path.Combine(OutputPath, Path.GetFileName(filename));
				CheckFileExists(destination);
				if (_canceled)
					return;

				File.Copy(filename, destination, true);
			}
		}

		private void CheckFileExists(string fileName)
		{
			if (_overwrite || !File.Exists(fileName))
				return;

			_synchronizationContext.Send(
				delegate
				{
					_canceled = DialogBoxAction.No == DesktopWindow.ShowMessageBox(SR.MessageConfirmOverwriteFiles, MessageBoxActions.YesNo);
				}, null);

			_overwrite = !_canceled;
		}

		private void DoExport(IBackgroundTaskContext context)
		{
			try
			{
				int i = 0;
				int fileCount = _files.Count;

				foreach (string filename in _files)
				{
					string message = String.Format(SR.MessageFormatExportingFiles, i + 1, fileCount);
					BackgroundTaskProgress progress = new BackgroundTaskProgress(i, fileCount, message);
					context.ReportProgress(progress);

					SaveFile(filename);
					
					if (_canceled || context.CancelRequested)
					{
						_canceled = true;
						context.Cancel();
						return;
					}

					i++;
				}

				context.Complete();
			}
			catch (Exception e)
			{
				context.Error(e);
			}
		}
	}
}
