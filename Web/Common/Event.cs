#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.Collections;

namespace Macro.Web.Common
{
	[DataContract(Namespace = Namespace.Value)]
    public abstract class Event
    {
        [DataMember(IsRequired = false)]
        public String Sender { get; set; }

        [DataMember(IsRequired = false)]
        public Guid SenderId { get; set; }

        [DataMember(IsRequired = true)]
        public Guid Identifier { get; set; }

        /// <summary>
        /// Indicates if this event can be sent together with other messages.
        /// </summary>
        public virtual bool AllowSendInBatch { get { return true; } }

        #region Equality

        public override bool Equals(object obj)
        {
			if (obj is Event)
				return Equals((Event)obj);

            return false;
        }

        public static bool operator ==(Event e1, Event e2)
        {
            return Object.Equals(e1, e2);
        }

        public static bool operator !=(Event e1, Event e2)
        {
            return !Object.Equals(e1, e2);
        }

        #region IEquatable<Event> Members

        public bool Equals(Event other)
        {
            return other.Identifier == Identifier;
        }

        #endregion
        #endregion

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
			return String.Format("{0}:{1} [Sender: {2}, ID={3}]", GetType().Name, Identifier, Sender, SenderId);
        }
	}

    [DataContract(Namespace = Namespace.Value)]
    public class EventSet : IEnumerable
    {
    	// TODO: Is it used?
        [DataMember(IsRequired = true)]
        public bool HasMorePending { get; set; }

        [DataMember(IsRequired = true)]
        public Event[] Events { get; set; }

		[DataMember(IsRequired = true)]
		public int Number { get; set; }

		[DataMember(IsRequired = true)]
		public Guid ApplicationId { get; set; }
		
		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return (Events ?? new Event[0]).GetEnumerator();
		}

		#endregion
	}
}