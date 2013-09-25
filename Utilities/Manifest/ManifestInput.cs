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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Macro.Utilities.Manifest
{
    /// <summary>
    /// Input file generated by <see cref="ManifestInputGenerationApplication"/> for <see cref="ManifestGenerationApplication"/>.
    /// </summary>
    [XmlRoot("ManifestInput", Namespace = "http://www.ClearCanvas.ca")]
    public class ManifestInput
    {
        #region Class definitions

        [XmlRoot("File")]
        public class InputFile
        {
            [XmlAttribute(AttributeName = "checksum", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Checksum { get; set; }

            [XmlAttribute(AttributeName = "ignore", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Ignore { get; set; }

            [XmlAttribute(AttributeName = "config", DataType = "boolean")]
            [DefaultValue(false)]
            public bool Config { get; set; }

            [XmlAttribute(AttributeName = "name", DataType = "string")]
            public string Name;
        }

        #endregion Class definitions

        #region Private Members

        private List<InputFile> _files;

        #endregion Private Members

        #region Public Properties

        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<InputFile> Files
        {
            get
            {
                if (_files == null)
                    _files = new List<InputFile>();
                return _files;
            }
            set { _files = value; }
        }

        #endregion Public Properties

        #region Public Static Methods

        public static ManifestInput Deserialize(string filename)
        {
            XmlSerializer theSerializer = new XmlSerializer(typeof (ManifestInput));

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                ManifestInput input = (ManifestInput) theSerializer.Deserialize(fs);

                return input;
            }
        }

        public static void Serialize(string filename, ManifestInput input)
        {
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                XmlSerializer theSerializer = new XmlSerializer(typeof (ManifestInput));

                XmlWriterSettings settings = new XmlWriterSettings
                                                 {
                                                     Indent = true,
                                                     IndentChars = "  ",
                                                     Encoding = Encoding.UTF8,
                                                 };

                XmlWriter writer = XmlWriter.Create(fs, settings);
                if (writer != null)
                    theSerializer.Serialize(writer, input);

                fs.Flush();
                fs.Close();
            }
        }

        #endregion
    }
}