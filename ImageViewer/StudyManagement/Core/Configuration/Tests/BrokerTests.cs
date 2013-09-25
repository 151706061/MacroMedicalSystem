﻿#region License

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

#if UNIT_TESTS

using System;
using Macro.Common.Configuration;
using NUnit.Framework;

namespace Macro.ImageViewer.StudyManagement.Core.Configuration.Tests
{
    [TestFixture]
    public class BrokerTests
    {
        private void DeleteAllDocuments()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationDocumentBroker();
                broker.DeleteAllDocuments();
            }
        }

        [Test]
        public void TestAddAndGetPrior()
        {
            DeleteAllDocuments();

            string documentName = "test";
            string instanceKey = null;
            string user = null;

            var oldestKey = new ConfigurationDocumentKey(documentName, new Version(3, 5, 21685, 22177), user, instanceKey);
            var oldest = new ConfigurationDocument
            {
                CreationTime = DateTime.Now,
                DocumentName = oldestKey.DocumentName,
                DocumentVersionString = VersionUtils.ToPaddedVersionString(oldestKey.Version, false, false),
                User = oldestKey.User,
                InstanceKey = oldestKey.InstanceKey,
                DocumentText = "oldest"
            };
            
            var previousKey = new ConfigurationDocumentKey(documentName, new Version(4, 4, 21685, 22177), user, instanceKey);
            var previous = new ConfigurationDocument
            {
                CreationTime = DateTime.Now,
                DocumentName = previousKey.DocumentName,
                DocumentVersionString = VersionUtils.ToPaddedVersionString(previousKey.Version, false, false),
                User = previousKey.User,
                InstanceKey = previousKey.InstanceKey,
                DocumentText = "previous"
            };

            var newestKey = new ConfigurationDocumentKey(documentName, new Version(5, 1, 21685, 22177), user, instanceKey);
            var newest = new ConfigurationDocument
            {
                CreationTime = DateTime.Now,
                DocumentName = newestKey.DocumentName,
                DocumentVersionString = VersionUtils.ToPaddedVersionString(newestKey.Version, false, false),
                User = newestKey.User,
                InstanceKey = newestKey.InstanceKey,
                DocumentText = "newest"
            };

            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationDocumentBroker();
                broker.AddConfigurationDocument(oldest);
                broker.AddConfigurationDocument(previous);
                broker.AddConfigurationDocument(newest);
                context.Commit();
            }

            using (var context = new DataAccessContext())
            {
                var broker = context.GetConfigurationDocumentBroker();
                var oldestRetrieved = broker.GetConfigurationDocument(oldestKey);
                Assert.AreEqual(oldestRetrieved.DocumentName, oldest.DocumentName);
                Assert.AreEqual(oldestRetrieved.DocumentVersionString, oldest.DocumentVersionString);

                var previousRetrieved = broker.GetConfigurationDocument(previousKey);
                Assert.AreEqual(previousRetrieved.DocumentName, previous.DocumentName);
                Assert.AreEqual(previousRetrieved.DocumentVersionString, previous.DocumentVersionString);

                var newestRetrieved = broker.GetConfigurationDocument(newestKey);
                Assert.AreEqual(newestRetrieved.DocumentName, newest.DocumentName);
                Assert.AreEqual(newestRetrieved.DocumentVersionString, newest.DocumentVersionString);

                var previousOfNewest = broker.GetPriorConfigurationDocument(newestKey);
                Assert.AreEqual(previousOfNewest.DocumentName, previous.DocumentName);
                Assert.AreEqual(previousOfNewest.DocumentVersionString, previous.DocumentVersionString);
            }
        }
    }
}

#endif