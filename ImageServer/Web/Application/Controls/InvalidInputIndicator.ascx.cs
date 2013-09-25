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
using System.Web.UI.WebControls;
using Macro.ImageServer.Web.Common.WebControls.Validators;

namespace Macro.ImageServer.Web.Application.Controls
{
    [Themeable(true)]
    public partial class InvalidInputIndicator : UserControl, IInvalidInputIndicator
    {
        private int _referenceCounter;

        public String ImageUrl
        {
            get { return Image.ImageUrl; }
            set { Image.ImageUrl = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ContainerPanel.Attributes.Add("shared", _referenceCounter>1? "true":"false");
            ContainerPanel.Attributes.Add("numberofinvalidfields", "0");
 
            ContainerPanel.Style.Add("display", "block");
            ContainerPanel.Style.Add("visibility", "hidden");   

        }
                
           
        public Control Container
        {
            get { return ContainerPanel; }
        }


        public void Show()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "visible";

        }

        public void Hide()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "hidden";
             
        }


        public Label TooltipLabel
        {
            get { return HintLabel; }
        }

        

        public void AttachValidator(Common.WebControls.Validators.BaseValidator validator)
        {
            _referenceCounter ++;
            validator.InputControl.Attributes.Add("multiplevalidators", _referenceCounter > 1 ? "true" : "false");
        }


        #region IInvalidInputIndicator Members


        public Control TooltipLabelContainer
        {
            get
            {
                return HintPanel;

            }
        }

        #endregion

        #region IInvalidInputIndicator ≥…‘±


        public bool IsVisible
        {
            get
            {
                return ContainerPanel.Style[HtmlTextWriterStyle.Visibility] == "visible";
            }
        }

        #endregion
    }
}