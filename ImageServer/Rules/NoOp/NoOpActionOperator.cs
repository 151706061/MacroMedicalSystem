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

using System.Xml;
using System.Xml.Schema;
using Macro.Common;
using Macro.Common.Actions;
using Macro.Dicom.Utilities.Rules;
using Macro.ImageServer.Model;

namespace Macro.ImageServer.Rules.NoOp
{
	[ExtensionOf(typeof(XmlActionCompilerOperatorExtensionPoint<ServerActionContext, ServerRuleTypeEnum>))]
	public class NoOpActionOperator : ActionOperatorCompilerBase, IXmlActionCompilerOperator<ServerActionContext, ServerRuleTypeEnum>
    {
        public NoOpActionOperator()
            : base("no-op")
        {
        }

        #region IXmlActionCompilerOperator<ServerActionContext> Members

        public IActionItem<ServerActionContext> Compile(XmlElement xmlNode)
        {
            return new NoOpActionItem();
        }

        public XmlSchemaElement GetSchema(ServerRuleTypeEnum ruleType)
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();

            XmlSchemaElement element = new XmlSchemaElement();
            element.Name = "no-op";
            element.SchemaType = type;

            return element;
        }

        #endregion
    }
}