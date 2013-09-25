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
using Macro.Enterprise.Core;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Model
{
    public partial class Study
    {
        #region Private Fields
        static private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();
        private IDictionary<string, Series> _series = null;
        private Patient _patient=null;
        #endregion

        #region Public Properties

        public bool HasAttachment
        {
            get
            {
                if (this.Series==null)
                    return false;

                return CollectionUtils.Contains(this.Series.Values, 
                    (series)=> !String.IsNullOrEmpty(series.Modality) && (series.Modality.Equals("SR") || series.Modality.Equals("DOC")));
            }
        }

        /// <summary>
        /// Gets the <see cref="Series"/> related to this study.
        /// </summary>
        public IDictionary<string, Series> Series
        {
            get
            {
                if (_series == null)
                {
                    lock (SyncRoot)
                    {
                        using (IReadContext readContext = _store.OpenReadContext())
                        {
                            ISeriesEntityBroker broker = readContext.GetBroker<ISeriesEntityBroker>();
                            SeriesSelectCriteria criteria = new SeriesSelectCriteria();
                            criteria.StudyKey.EqualTo(this.GetKey());
                            IList<Series> list = broker.Find(criteria);

                            _series = new Dictionary<string, Series>();
                            foreach(Series theSeries in list)
                            {
                                _series.Add(theSeries.SeriesInstanceUid, theSeries);
                            }
                        }
                    }
                }

                return _series;
            }
        }

        /// <summary>
        /// Gets the <see cref="Patient"/> related to this study
        /// </summary>
        public Patient Patient
        {
            get
            {
                if (_patient==null)
                {
                    lock (SyncRoot)
                    {
                        using (IReadContext readContext = _store.OpenReadContext())
                        {
                            _patient = Model.Patient.Load(this.PatientKey);
                        }
                    }
                }
                return _patient;
            }
        }

        #endregion

        /// <summary>
        /// Find a <see cref="Study"/> with the specified study instance uid on the given partition.
        /// </summary>
        /// <param name="studyInstanceUid"></param>
        /// <param name="partition"></param>
        /// <returns></returns>
        /// 
        static public Study Find(IPersistenceContext context, String studyInstanceUid, ServerPartition partition)
        {
            IStudyEntityBroker broker = context.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partition.GetKey());
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            Study study = broker.FindOne(criteria);
            return study;
           
        }

        public Patient LoadPatient(IPersistenceContext context)
        {
            if (_patient==null)
            {
                lock (SyncRoot)
                {
                    if (_patient == null)
                    {
                        _patient = Patient.Load(context, PatientKey);
                    }
                }
            }
            return _patient;
        }


        public StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            return StudyStorage.Load(this.StudyStorageKey);
        }

        static public Study Find(IPersistenceContext context, ServerEntityKey studyStorageKey)
        {
            IStudyEntityBroker broker = context.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.StudyStorageKey.EqualTo( studyStorageKey);
            return broker.FindOne(criteria);
        }
    }
}
