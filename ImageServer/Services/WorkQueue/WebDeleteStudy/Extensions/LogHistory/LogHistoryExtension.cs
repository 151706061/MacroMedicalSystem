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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Enterprise.Core;
using Macro.ImageServer.Common.Utilities;
using Macro.ImageServer.Core.Data;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory
{
    [ExtensionOf(typeof(WebDeleteProcessorExtensionPoint))]
    class LogHistoryExtension:IWebDeleteProcessorExtension
    {
        private StudyInformation _studyInfo;
        
        #region IWebDeleteProcessorExtension Members

        public void OnSeriesDeleting(WebDeleteProcessorContext context, Series series)
        {
            _studyInfo = StudyInformation.CreateFrom(context.StorageLocation.Study);
        }

        public void OnSeriesDeleted(WebDeleteProcessorContext context, Series _series)
        {
            
        }

        #endregion

        #region IWebDeleteProcessorExtension Members


        public void OnCompleted(WebDeleteProcessorContext context, IList<Series> series)
        {
            if (series.Count > 0)
            {
                Platform.Log(LogLevel.Info, "Logging history..");
                DateTime now = Platform.Time;
                using(IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IStudyHistoryEntityBroker broker = ctx.GetBroker<IStudyHistoryEntityBroker>();
                    StudyHistoryUpdateColumns columns = new StudyHistoryUpdateColumns();
                    columns.InsertTime = Platform.Time;
                    columns.StudyHistoryTypeEnum = StudyHistoryTypeEnum.SeriesDeleted;
                    columns.StudyStorageKey = context.StorageLocation.Key;
                    columns.DestStudyStorageKey = context.StorageLocation.Key;
                    columns.StudyData = XmlUtils.SerializeAsXmlDoc(_studyInfo);
                    SeriesDeletionChangeLog changeLog =  new SeriesDeletionChangeLog();
                    changeLog.TimeStamp = now;
                    changeLog.Reason = context.Reason;
                    changeLog.UserId = context.UserId;
                    changeLog.UserName = context.UserName;
                    changeLog.Series = CollectionUtils.Map(series,
                                      delegate(Series ser)
                                          {
                                              ServerEntityAttributeProvider seriesWrapper = new ServerEntityAttributeProvider(ser);
                                              return new SeriesInformation(seriesWrapper);
                                          });
                    columns.ChangeDescription = XmlUtils.SerializeAsXmlDoc(changeLog);
                    StudyHistory history = broker.Insert(columns);
                    if (history!=null)
                        ctx.Commit();
                    
                }
            }
            
        }

        #endregion
    }

    public class SeriesDeletionChangeLog
    {
        private DateTime _timeStamp;
        private string _userId;
        private string _userName;
        private string _reason;
        private List<SeriesInformation> _seriesInfo;
        
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
        [XmlArray("DeletedSeries")]
        public List<SeriesInformation> Series
        {
            get { return _seriesInfo; }
            set { _seriesInfo = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
    }
}
