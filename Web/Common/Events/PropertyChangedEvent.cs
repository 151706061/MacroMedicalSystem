#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace Macro.Web.Common.Events
{
	[DataContract(Namespace = Namespace.Value)]
	public class PropertyChangedEvent : Event
	{
		[DataMember(IsRequired = false)]
		public string PropertyName { get; set; }

		[DataMember(IsRequired = false)]
		public object Value { get; set; }

        [DataMember(IsRequired = false)]
        public string[] DebugInfo { get; set; }

        public override string ToString()
        {
            return String.Format("{0}, <Property:{1}[{2}]>", base.ToString(), PropertyName, ToStringHelper(Value));
        }

		private static string ToStringHelper(object value)
		{
			if (value == null)
				return null;

			if (value is ICollection)
			{
				ICollection collection = (ICollection)value;
				return String.Format("Count={0}", collection.Count);
			}

			return value.ToString();
		}
	}
}