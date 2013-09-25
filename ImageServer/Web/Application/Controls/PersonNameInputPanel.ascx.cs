#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.UI;
using Macro.Common.Utilities;
using Macro.Dicom.Iod;
using Macro.ImageServer.Web.Common.WebControls;

[assembly: WebResource("Macro.ImageServer.Web.Application.Controls.PersonNameInputPanel.js", "application/x-javascript")]

namespace Macro.ImageServer.Web.Application.Controls
{
    public partial class PersonNameInputPanel : UserControl
    {
        private PersonName _personName;
        private string _validationGroup = "PersonNameInputValidationGroup";
        public PersonName PersonName
        {
            set { _personName = value; }
            get
            {
                string singlebyte =
                    StringUtilities.Combine(new[]
                                                        {
                                                            PersonLastName.Text,
                                                            PersonGivenName.Text,
                                                            PersonMiddleName.Text,
                                                            PersonTitle.Text,
                                                            PersonSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                singlebyte = singlebyte.TrimEnd('^');

                string ideographicName =
                    StringUtilities.Combine(new[]
                                                        {
                                                            IdeographicLastName.Text,
                                                            IdeographicGivenName.Text,
                                                            IdeographicMiddleName.Text,
                                                            IdeographicTitle.Text,
                                                            IdeographicSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                ideographicName = ideographicName.TrimEnd('^');

                string phoneticName =
                    StringUtilities.Combine(new[]
                                                        {
                                                            PhoneticLastName.Text,
                                                            PhoneticGivenName.Text,
                                                            PhoneticMiddleName.Text,
                                                            PhoneticTitle.Text,
                                                            PhoneticSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                phoneticName = phoneticName.TrimEnd('^');


                string dicomName = StringUtilities.Combine(new[]
                                                        {
                                                            singlebyte,
                                                            ideographicName,
                                                            phoneticName
                                                        }, "=", false);

                dicomName = dicomName.TrimEnd('=');

                return new PersonName(dicomName);
            }
        }

        public bool Required { get; set; }

        public string ValidationGroup
        {
            get
            {
                return _validationGroup;
            }
            set { _validationGroup = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PhoneticNameRowIndicator.ImageUrl = ImageServerConstants.ImageURLs.PhoneticName;
            IdeographyNameIndicator.ImageUrl = ImageServerConstants.ImageURLs.IdeographyName;
            var script =
                new ScriptTemplate(typeof(PersonNameInputPanel).Assembly,
                                   "Macro.ImageServer.Web.Application.Controls.PersonNameInputPanel.js");
            script.Replace("@@CLIENTID@@", ClientID);
            script.Replace("@@PHONETIC_ROW_CLIENTID@@", PhoneticRow.ClientID);
            script.Replace("@@IDEOGRAPHY_ROW_CLIENTID@@", IdeographicRow.ClientID);


            ShowOtherNameFormatButton.OnClientClick = ClientID + "_ShowOtherNameFormats(); return false;";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script.Script, true);

            PersonGivenNameValidator.IgnoreEmptyValue = !Required;
            PersonLastNameValidator.IgnoreEmptyValue = !Required;

            //Dynamically set all of the Validation Groups
            PersonTitle.ValidationGroup = ValidationGroup;
            PersonTitleValidator.ValidationGroup = ValidationGroup;
            PersonGivenName.ValidationGroup = ValidationGroup;
            PersonGivenNameValidator.ValidationGroup = ValidationGroup;
            PersonMiddleName.ValidationGroup = ValidationGroup;
            PersonMiddleNameValidator.ValidationGroup = ValidationGroup;
            PersonLastName.ValidationGroup = ValidationGroup;
            PersonLastNameValidator.ValidationGroup = ValidationGroup;
            PersonSuffix.ValidationGroup = ValidationGroup;
            PersonSuffixValidator.ValidationGroup = ValidationGroup;
            PhoneticTitle.ValidationGroup = ValidationGroup;
            PhoneticGivenName.ValidationGroup = ValidationGroup;
            PhoneticMiddleName.ValidationGroup = ValidationGroup;
            PhoneticLastName.ValidationGroup = ValidationGroup;
            PhoneticSuffix.ValidationGroup = ValidationGroup;
            IdeographicTitle.ValidationGroup = ValidationGroup;
            IdeographicGivenName.ValidationGroup = ValidationGroup;
            IdeographicMiddleName.ValidationGroup = ValidationGroup;
            IdeographicLastName.ValidationGroup = ValidationGroup;
            IdeographicSuffix.ValidationGroup = ValidationGroup;
        }

        public override void DataBind()
        {
            base.DataBind();

            if (_personName != null)
            {
                PersonLastName.Text = _personName.LastName;
                PersonMiddleName.Text = _personName.MiddleName;
                PersonGivenName.Text = _personName.FirstName;
                PersonTitle.Text = _personName.Title;
                PersonSuffix.Text = _personName.Suffix;

                if (_personName.Phonetic.IsEmpty)
                {
                    //PhoneticRow.Visible = false;
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    PhoneticLastName.Text = _personName.Phonetic.FamilyName;
                    PhoneticGivenName.Text = _personName.Phonetic.GivenName;
                    PhoneticMiddleName.Text = _personName.Phonetic.MiddleName;
                    PhoneticTitle.Text = _personName.Phonetic.Prefix;
                    PhoneticSuffix.Text = _personName.Phonetic.Suffix;
                }

                if (_personName.Ideographic.IsEmpty)
                {
                    //IdeographicRow.Visible = false;

                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    IdeographicLastName.Text = _personName.Ideographic.FamilyName;
                    IdeographicGivenName.Text = _personName.Ideographic.GivenName;
                    IdeographicMiddleName.Text = _personName.Ideographic.MiddleName;
                    IdeographicTitle.Text = _personName.Ideographic.Prefix;
                    IdeographicSuffix.Text = _personName.Ideographic.Suffix;
                }
            }
            else
            {
                // only show the single byte row
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
        }
    }
}