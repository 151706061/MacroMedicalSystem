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

namespace Macro.Common.Actions
{
    /// <summary>
    /// Interface for extensions implementing <see cref="XmlActionCompilerOperatorExtensionPoint{TActionContext,TSchemaContext}"/>.
    /// </summary>
    public interface IXmlActionCompilerOperator<TActionContext, TSchemaContext>
    {
        /// <summary>
        /// The name of the action implemented.  This is typically the name of the <see cref="XmlElement"/> describing the action.
        /// </summary>
        string OperatorTag { get; }

        /// <summary>
        /// Method used to compile the action.  
        /// </summary>
        /// <param name="xmlNode">Input <see cref="XmlElement"/> describing the action to perform.</param>
        /// <returns>A class implementing the <see cref="IActionItem{T}"/> interface which can perform the action.</returns>
        IActionItem<TActionContext> Compile(XmlElement xmlNode);

        /// <summary>
        /// Get an <see cref="XmlSchemaElement"/> describing the ActionItem for validation purposes.
        /// </summary>
        /// <param name="context">A context in which the schema is being generated.</param>
        /// <returns></returns>
        XmlSchemaElement GetSchema(TSchemaContext context);
    }
}