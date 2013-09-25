#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using Macro.Common;
using Macro.ImageServer.Rules;

namespace Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
{
    /// <summary>
    /// Summary description for ServerRuleSamples
    /// </summary>
    [WebService(Namespace = "http://www.ClearCanvas.ca/ImageServer/ServerRuleSamples.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ServerRuleSamples : WebService
    {
        [WebMethod]
        public string GetXml(string type)
        {
            try
            {
                XmlDocument doc = null;

                string inputString = Server.HtmlEncode(type);
                if (String.IsNullOrEmpty(inputString))
                    inputString = string.Empty;

                var ep = new SampleRuleExtensionPoint();
                object[] extensions = ep.CreateExtensions();

                foreach (ISampleRule extension in extensions)
                {
                    if (extension.Name.Equals(inputString))
                    {
                        doc = extension.Rule;
                        break;
                    }
                }

                if (doc == null)
                {
                    doc = new XmlDocument();
                    XmlNode node = doc.CreateElement("rule");
                    doc.AppendChild(node);
                    XmlElement conditionNode = doc.CreateElement("condition");
                    node.AppendChild(conditionNode);
                    conditionNode.SetAttribute("expressionLanguage", "dicom");
                    XmlNode actionNode = doc.CreateElement("action");
                    node.AppendChild(actionNode);
                }

                var sw = new StringWriter();

                var xmlSettings = new XmlWriterSettings
                                      {
                                          Encoding = Encoding.UTF8,
                                          ConformanceLevel = ConformanceLevel.Fragment,
                                          Indent = true,
                                          NewLineOnAttributes = false,
                                          CheckCharacters = true,
                                          IndentChars = "  "
                                      };

                XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

                if (tw != null)
                {
                    doc.WriteTo(tw);
                    tw.Close();
                }

                return sw.ToString();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception processing server rule of type: {0}", type);
                throw;
            }
        }
    }
}