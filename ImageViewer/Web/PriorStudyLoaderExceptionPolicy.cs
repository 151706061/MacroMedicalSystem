#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using Macro.Common;
using Macro.Desktop;

namespace Macro.ImageViewer.Web
{
    [ExceptionPolicyFor(typeof(LoadPriorStudiesException))]
    [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public class PriorStudyLoaderExceptionPolicy : IExceptionPolicy
    {
        public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
        {
            if (e is LoadPriorStudiesException)
            {
                exceptionHandlingContext.Log(LogLevel.Error, e);

                Handle(e as LoadPriorStudiesException, exceptionHandlingContext);
            }
        }

        private static void Handle(LoadPriorStudiesException exception, IExceptionHandlingContext context)
        {
            if (exception.FindFailed)
            {
                context.ShowMessageBox(SR.MessageLoadPriorsFindErrors);
                return;
            }
            
            var summary = new StringBuilder();
            if (!exception.FindResultsComplete)
                summary.Append(SR.MessagePriorsIncomplete);

            if (ShouldShowLoadErrorMessage(exception))
            {
                if (summary.Length > 0)
                {
                    summary.AppendLine(); summary.AppendLine("----"); summary.AppendLine();
                }

                summary.Append(Macro.Web.Services.ExceptionTranslator.Translate(exception));
            }

            if (summary.Length > 0)
                context.ShowMessageBox(summary.ToString());
        }

        private static bool ShouldShowLoadErrorMessage(LoadPriorStudiesException exception)
        {
            if (exception.IncompleteCount > 0)
                return true;

            if (exception.NotFoundCount > 0)
                return true;

            if (exception.UnknownFailureCount > 0)
                return true;

            return false;
        }
    }
}
