#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Web.Common.Data
{
    internal class ServerRuleAdaptor :
        BaseAdaptor
            <ServerRule, IServerRuleEntityBroker, ServerRuleSelectCriteria, ServerRuleUpdateColumns>
    {
    }

    public class ServerRuleController : BaseController
    {
        #region Private Members

        private readonly ServerRuleAdaptor _adaptor = new ServerRuleAdaptor();

        #endregion

        #region Public Methods

        public IList<ServerRule> GetServerRules(ServerRuleSelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }

        public bool DeleteServerRule(ServerRule rule)
        {
            return _adaptor.Delete(rule.Key);
        }

        public ServerRule AddServerRule(ServerRule rule)
        {
            ServerRuleUpdateColumns parms = new ServerRuleUpdateColumns();

            parms.DefaultRule = rule.DefaultRule;
            parms.Enabled = rule.Enabled;
            parms.RuleName = rule.RuleName;
            parms.RuleXml = rule.RuleXml;
            parms.ServerPartitionKey = rule.ServerPartitionKey;
            parms.ServerRuleApplyTimeEnum = rule.ServerRuleApplyTimeEnum;
            parms.ServerRuleTypeEnum = rule.ServerRuleTypeEnum;
        	parms.ExemptRule = rule.ExemptRule;

            return _adaptor.Add(parms);
        }

        public bool UpdateServerRule(ServerRule rule)
        {
            ServerRuleUpdateColumns parms = new ServerRuleUpdateColumns();

            parms.DefaultRule = rule.DefaultRule;
            parms.Enabled = rule.Enabled;
            parms.RuleName = rule.RuleName;
            parms.RuleXml = rule.RuleXml;
            parms.ServerPartitionKey = rule.ServerPartitionKey;
            parms.ServerRuleApplyTimeEnum = rule.ServerRuleApplyTimeEnum;
            parms.ServerRuleTypeEnum = rule.ServerRuleTypeEnum;
			parms.ExemptRule = rule.ExemptRule;

            return _adaptor.Update(rule.Key, parms);
        }

        #endregion
    }
}
