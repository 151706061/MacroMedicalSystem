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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Macro.ImageViewer.Utilities.Media.PortableViewer
{


    [Serializable, DesignerCategory("code"), XmlType(AnonymousType=true), DebuggerStepThrough, GeneratedCode("xsd", "2.0.50727.3038")]
    public sealed class MediaFileSetPrivateInformation
    {
        private XmlElement anyField;
        private string creatorUIDField;

        [XmlAnyElement]
        public XmlElement Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        [XmlAttribute]
        public string CreatorUID
        {
            get
            {
                return this.creatorUIDField;
            }
            set
            {
                this.creatorUIDField = value;
            }
        }
    }
}

