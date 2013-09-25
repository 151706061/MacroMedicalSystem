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
using Macro.Common;
using Macro.Dicom;
using Macro.ImageViewer.StudyManagement;

namespace Macro.ImageViewer.Utilities.StudyFilters
{
	//TODO (CR February 2011) - High: These don't get disposed - causes caching problems.
	public class SopDataSourceStudyItem : StudyItem
	{
		private readonly string _filename;
		private ISopReference _sopReference;

		public SopDataSourceStudyItem(Sop sop)
		{
			if (sop.DataSource is ILocalSopDataSource)
			{
				_filename = ((ILocalSopDataSource) sop.DataSource).Filename;
				_sopReference = sop.CreateTransientReference();
			}
		}

		public SopDataSourceStudyItem(ILocalSopDataSource sopDataSource)
		{
			_filename = sopDataSource.Filename;
			using (Sop sop = new Sop(sopDataSource))
			{
				_sopReference = sop.CreateTransientReference();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sopReference != null)
				{
					_sopReference.Dispose();
					_sopReference = null;
				}
			}
			base.Dispose(disposing);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get { return _sopReference.Sop[tag]; }
		}
	}

	public class LocalStudyItem : StudyItem
	{
		private readonly string _filename;
		private readonly DicomFile _dcf;

		public LocalStudyItem(string filename)
		{
			_filename = filename;
			_dcf = new DicomFile(filename);
			_dcf.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		public override string Filename
		{
			get { return _filename; }
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (!_dcf.DataSet.TryGetAttribute(tag, out attribute))
				{
					if (!_dcf.MetaInfo.TryGetAttribute(tag, out attribute))
						return null;
				}
				return attribute;
			}
		}
	}

	public abstract class StudyItem : IStudyItem
	{
		protected StudyItem() {}

		~StudyItem()
		{
			this.Dispose(false);
		}

		public abstract string Filename { get; }

		public abstract DicomAttribute this[uint tag] { get; }

		protected virtual void Dispose(bool disposing) {}

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}
	}
}