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
using System.IO;
using System.Threading;
using System.Xml;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Dicom;
using Macro.Dicom.Network;
using Macro.Dicom.Network.Scp;
using Macro.Dicom.Utilities.Xml;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common;
using Macro.ImageServer.Common.Helpers;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.Brokers;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.ImageServer.Model.Parameters;

namespace Macro.ImageServer.Services.Dicom
{
    /// <summary>
    /// Base class for all DicomScpExtensions for the ImageServer.
    /// </summary>
    public abstract class BaseScp : IDicomScp<DicomScpContext>
    {

        #region Protected Members
        protected IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
    	private DicomScpContext _context;
        private Device _device;
        #endregion

        #region Private Methods

        #endregion

        #region Properties
        /// <summary>
		/// The <see cref="ServerPartition"/> associated with the Scp.
		/// </summary>
        protected ServerPartition Partition
        {
            get { return _context.Partition; }
        }

        protected Device Device
        {
            get { return _device; }
            set { _device = value; }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Called during association verification.
        /// </summary>
        /// <param name="association"></param>
        /// <param name="pcid"></param>
        /// <returns></returns>
        protected abstract DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid);

        #endregion

        #region Public Methods

		/// <summary>
		/// Verify a presentation context.
		/// </summary>
		/// <param name="association"></param>
		/// <param name="pcid"></param>
		/// <returns></returns>
        public DicomPresContextResult VerifyAssociation(AssociationParameters association, byte pcid)
        {
            bool isNew;

            Device = DeviceManager.LookupDevice(Partition, association, out isNew);

            // Let the subclass perform the verification
            DicomPresContextResult result = OnVerifyAssociation(association, pcid);
            if (result!=DicomPresContextResult.Accept)
            {
                Platform.Log(LogLevel.Debug, "Rejecting Presentation Context {0}:{1} in association between {2} and {3}.",
                             pcid, association.GetAbstractSyntax(pcid).Description,
                             association.CallingAE, association.CalledAE);
            }

            return result;
        }

        
        /// <summary>
        /// Helper method to load a <see cref="StudyXml"/> instance for a given study location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            StudyXml theXml = new StudyXml();

            if (File.Exists(streamFile))
            {
                // allocate the random number generator here, in case we need it below
                Random rand = new Random();

                // Go into a retry loop, to handle if the study is being processed right now 
                for (int i = 0; ;i++ )
                    try
                    {
                        using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                        {
                            XmlDocument theDoc = new XmlDocument();

                            StudyXmlIo.Read(theDoc, fileStream);

                            theXml.SetMemento(theDoc);

                            fileStream.Close();

                            return theXml;
                        }
                    }
                    catch (IOException)
                    {
                        if (i < 5)
                        {
                            Thread.Sleep(rand.Next(5, 50)); // Sleep 5-50 milliseconds
                            continue;
                        }

                        throw;
                    }
            }

            return theXml;
        }

		/// <summary>
		/// Get the Status of a study.
		/// </summary>
		/// <param name="studyInstanceUid">The Study to check for.</param>
		/// <param name="studyStorage">The returned study storage object</param>
		/// <returns>true on success, false on no records found.</returns>
		public bool GetStudyStatus(string studyInstanceUid, out StudyStorage studyStorage)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				IStudyStorageEntityBroker selectBroker = read.GetBroker<IStudyStorageEntityBroker>();
				StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();

				criteria.ServerPartitionKey.EqualTo(Partition.GetKey());
				criteria.StudyInstanceUid.EqualTo(studyInstanceUid);

				IList<StudyStorage> storageList = selectBroker.Find(criteria);

				foreach (StudyStorage studyLocation in storageList)
				{
					studyStorage = studyLocation;
					return true;
				}
				studyStorage = null;
				return false;
			}
		}

        #endregion

        #region IDicomScp Members

        public void SetContext(DicomScpContext parms)
        {
        	_context = parms;
        }

        public virtual void Cleanup()
        {
        }

        public virtual bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

        public virtual IList<SupportedSop> GetSupportedSopClasses()
        {
            throw new Exception("The method or operation is not implemented.  The method must be overriden.");
        }

        #endregion
    }
}