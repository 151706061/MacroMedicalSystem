#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LosslessAction
{
	/// <summary>
	/// JPEG 2000 Lossless action item for <see cref="ServerRulesEngine"/>
	/// </summary>
	public class Jpeg2000LosslessActionItem : ServerActionItemBase
	{
		private static readonly FilesystemQueueTypeEnum _queueType = FilesystemQueueTypeEnum.LosslessCompress;
		private readonly Expression _exprScheduledTime;
		private readonly int _offsetTime;
		private readonly TimeUnit _units;

		public Jpeg2000LosslessActionItem(int time, TimeUnit unit)
			: this(time, unit, null)
		{
		}

		public Jpeg2000LosslessActionItem(int time, TimeUnit unit, Expression exprScheduledTime)
			: base("JPEG 2000 Lossless compression action")
		{
			_offsetTime = time;
			_units = unit;
			_exprScheduledTime = exprScheduledTime;
		}

		protected override bool OnExecute(ServerActionContext context)
		{
			DateTime scheduledTime = Platform.Time;

			if (_exprScheduledTime != null)
			{
				scheduledTime = Evaluate(_exprScheduledTime, context, scheduledTime);
			}

			scheduledTime = CalculateOffsetTime(scheduledTime, _offsetTime, _units);
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("compress");
			doc.AppendChild(element);
			XmlAttribute syntaxAttribute = doc.CreateAttribute("syntax");
			syntaxAttribute.Value = TransferSyntax.Jpeg2000ImageCompressionLosslessOnlyUid;
			element.Attributes.Append(syntaxAttribute);

			Platform.Log(LogLevel.Debug, "Jpeg 2000 Lossless Compression Scheduling: This study will be compressed on {0}", scheduledTime);
			context.CommandProcessor.AddCommand(
				new InsertFilesystemQueueCommand(_queueType, context.FilesystemKey, context.StudyLocationKey,
				                                 scheduledTime, doc));

			return true;
		}

	}
}