#region License

//HL7 Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.HL7.Brokers;

namespace ClearCanvas.HL7.In
{
    /// <summary>
    /// Responsible for processing a single LogicalHL7Event into one or more HL7Event objects per peer
    /// </summary>
    public class HL7MessageInboundProcessor : EntityQueueProcessor<HL7Message>
    {

        internal HL7MessageInboundProcessor(int batchSize, TimeSpan sleepTime)
            : base(batchSize, sleepTime)
        {
        }

        protected override IList<HL7Message> GetNextEntityBatch(int batchSize)
        {
            var criteria = new HL7MessageSearchCriteria();
            criteria.Direction.EqualTo("I");
            criteria.Status.EqualTo("P");
            criteria.CreationTime.SortAsc(0);
            return PersistenceScope.CurrentContext.GetBroker<IHL7MessageBroker>().Find(criteria,
                                                                                       new SearchResultPage(0, batchSize));
        }

        protected override void MarkItemClaimed(HL7Message item)
        {
            //no-op
        }

        protected override void ActOnItem(HL7Message item)
        {
            foreach (IIncomingHL7MessageListener listener in new IncomingHL7MessageListenerExtensionPoint().CreateExtensions())
            {
                listener.OnMessage(item);
            }
        }

        protected override void OnItemSucceeded(HL7Message item)
        {
            item.Status = "C";
        }

        protected override void OnItemFailed(HL7Message item, Exception error)
        {
            item.Status = "E";
        }
    }
}