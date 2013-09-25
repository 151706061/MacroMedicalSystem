#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using System.Web.UI.WebControls;

namespace Macro.ImageServer.Web.Common.WebControls.Validators
{
    public interface IInvalidInputIndicator
    {
        Control Container { get; }

        Label TooltipLabel { get; }

        Control TooltipLabelContainer { get; }


        void AttachValidator(BaseValidator validator);

        void Show();
        void Hide();
    }
}