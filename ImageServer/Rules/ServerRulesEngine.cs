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
using System.Xml;
using Macro.Common;
using Macro.Common.Actions;
using Macro.Common.Specifications;
using Macro.Common.Utilities;
using Macro.Dicom.Utilities.Rules;
using Macro.Enterprise.Core;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;

namespace Macro.ImageServer.Rules
{
    /// <summary>
    /// Rules engine for applying rules against DICOM files and performing actions.
    /// </summary>
    /// <remarks>
    /// The ServerRulesEngine encapsulates code to apply rules against DICOM file 
    /// objects.  It will load the rules from the persistent store, maintain them by type,
    /// and then can apply them against specific files.
    /// </remarks>
    /// <seealso cref="ServerActionContext"/>
    /// <example>
    /// Here is an example rule for routing all images with Modality set to CT to an AE
    /// Title Macro.
    /// <code>
    /// <rule id="CT Rule">
    ///   <condition expressionLanguage="dicom">
    ///     <equal test="$Modality" refValue="CT"/>
    ///   </condition>
    ///   <action>
    ///     <auto-route device="Macro"/>
    ///   </action>
    /// </rule>
    /// </code>
    /// </example>
    public class ServerRulesEngine : RulesEngine<ServerActionContext,ServerRuleTypeEnum>
    {
        private readonly ServerRuleApplyTimeEnum _applyTime;
        private readonly ServerEntityKey _serverPartitionKey;

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// A rules engine will only load rules that apply at a specific time.  The
        /// apply time is specified by the <paramref name="applyTime"/> parameter.
        /// </remarks>
        /// <param name="applyTime">An enumerated value as to when the rules shall apply.</param>
        /// <param name="serverPartitionKey">The Server Partition the rules engine applies to.</param>
        public ServerRulesEngine(ServerRuleApplyTimeEnum applyTime, ServerEntityKey serverPartitionKey)
        {
            _applyTime = applyTime;
            _serverPartitionKey = serverPartitionKey;
            Statistics = new RulesEngineStatistics(applyTime.Lookup, applyTime.LongDescription);
        }

		#endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="ServerRuleApplyTimeEnum"/> for the rules engine.
        /// </summary>
        public ServerRuleApplyTimeEnum RuleApplyTime
        {
            get { return _applyTime; }
        }

        #endregion

        #region Public Methods

	
        /// <summary>
        /// Load the rules engine from the Persistent Store and compile the conditions and actions.
        /// </summary>
        public void Load()
        {
            Statistics.LoadTime.Start();

            // Clearout the current type list.
            _typeList.Clear();

            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IServerRuleEntityBroker broker = read.GetBroker<IServerRuleEntityBroker>();

                ServerRuleSelectCriteria criteria = new ServerRuleSelectCriteria();
                criteria.Enabled.EqualTo(true);
                criteria.ServerRuleApplyTimeEnum.EqualTo(_applyTime);
                criteria.ServerPartitionKey.EqualTo(_serverPartitionKey);

				// Add ommitted or included rule types, as appropriate
				if (_omitList.Count > 0)
					criteria.ServerRuleTypeEnum.NotIn(_omitList.ToArray());
				else if (_includeList.Count > 0)
					criteria.ServerRuleTypeEnum.In(_includeList.ToArray());

            	IList<ServerRule> list = broker.Find(criteria);

                // Create the specification and action compilers
                // We'll compile the rules right away
                XmlSpecificationCompiler specCompiler = new XmlSpecificationCompiler("dicom");
                XmlActionCompiler<ServerActionContext, ServerRuleTypeEnum> actionCompiler = new XmlActionCompiler<ServerActionContext, ServerRuleTypeEnum>();

                foreach (ServerRule serverRule in list)
                {
                    try
                    {
                        Rule<ServerActionContext, ServerRuleTypeEnum> theRule = new Rule<ServerActionContext, ServerRuleTypeEnum>();
                        theRule.Name = serverRule.RuleName;
                    	theRule.IsDefault = serverRule.DefaultRule;
                    	theRule.IsExempt = serverRule.ExemptRule;
                        theRule.Description = serverRule.ServerRuleApplyTimeEnum.Description;

                        XmlNode ruleNode =
                            CollectionUtils.SelectFirst<XmlNode>(serverRule.RuleXml.ChildNodes,
                                                                 delegate(XmlNode child) { return child.Name.Equals("rule"); });


						theRule.Compile(ruleNode, serverRule.ServerRuleTypeEnum, specCompiler, actionCompiler);

                        RuleTypeCollection<ServerActionContext, ServerRuleTypeEnum> typeCollection;

                        if (!_typeList.ContainsKey(serverRule.ServerRuleTypeEnum))
                        {
                            typeCollection = new RuleTypeCollection<ServerActionContext, ServerRuleTypeEnum>(serverRule.ServerRuleTypeEnum);
                            _typeList.Add(serverRule.ServerRuleTypeEnum, typeCollection);
                        }
                        else
                        {
                            typeCollection = _typeList[serverRule.ServerRuleTypeEnum];
                        }

                        typeCollection.AddRule(theRule);
                    }
                    catch (Exception e)
                    {
                        // something wrong with the rule...
                        Platform.Log(LogLevel.Warn, e, "Unable to add rule {0} to the engine. It will be skipped",
                                     serverRule.RuleName);
                    }
                }
            }

            Statistics.LoadTime.End();
        }

        #endregion
    }
}