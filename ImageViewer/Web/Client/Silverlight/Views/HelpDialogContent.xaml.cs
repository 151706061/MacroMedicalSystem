#region License

// Copyright (c) 2011, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.Macro.ca/OSLv3.0

#endregion

using System.Windows.Controls;
using Macro.ImageViewer.Web.Client.Silverlight.Resources;

namespace Macro.ImageViewer.Web.Client.Silverlight.Views
{
    public partial class HelpDialogContent : UserControl
    {

        const string Address = 
@"Macro Inc.
1920-439 University Ave.
Toronto, ON M5G 1Y8
Canada";
        const string PartOfCCRISPACS = "Part of the Macro RIS/PACS";
        const string WebAddress="www.Macro.ca";
        const string Copyright = "Copyright (c) 2012, Macro Inc. All rights reserved.";

        public HelpDialogContent()
        {
            InitializeComponent();

            ProductInfoTextBlock.Text = string.Format("{0}\n{1}", ApplicationContext.Current.ViewerVersion, PartOfCCRISPACS);
            CompanyAddressTextBlock.Text = Address;
            CompanyWebAddressTextBlock.Text = WebAddress;
            CopyrightTextBlock.Text = Copyright;
        }        
    }
}
