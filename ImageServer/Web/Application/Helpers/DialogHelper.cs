#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.ImageServer.Model;
using Macro.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace Macro.ImageServer.Web.Application.Helpers
{
	public class DialogHelper
	{
		public static string createConfirmationMessage(string message)
		{
			return string.Format("<span class=\"ConfirmDialogMessage\">{0}</span>", message);
		}

		public static string createStudyTable(IList<Study> studies)
		{
			string message;

			message =
				string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td>{2}</td></tr>",
                    ColumnHeaders.PatientName, ColumnHeaders.AccessionNumber, ColumnHeaders.StudyDescription);


			int i = 0;
			foreach (Study study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}

        // TODO: this looks very bad. Fix it
        public static string createSeriesTable(IList<Series> series)
        {
            string message;

            message =
                string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td colspan=\"2\">{2}</td><td colspan=\"2\">{3}</td></tr>",
                    ColumnHeaders.Modality, ColumnHeaders.SeriesDescription, ColumnHeaders.SeriesCount, ColumnHeaders.SeriesInstanceUID);
            
            int i = 0;
            foreach (Series s in series)
            {
                String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}</td><td>&nbsp;</td><td>{2}</td><td>&nbsp;</td><td>{3}</td></tr>",
                                 s.Modality, s.SeriesDescription, s.NumberOfSeriesRelatedInstances, s.SeriesInstanceUid);
                message += text;

                i++;
            }
            message += "</table>";

            return message;
        }

        // TODO: this looks very bad. Fix it
        public static string createStudyTable(IList<StudySummary> studies)
		{
			string message;

			message =
				string.Format("<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">{0}</td><td colspan=\"2\">{1}</td><td>{2}</td></tr>",
                    ColumnHeaders.PatientName, ColumnHeaders.AccessionNumber, ColumnHeaders.StudyDescription);

			int i = 0;
			foreach (StudySummary study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}&nbsp;</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}
	}
}