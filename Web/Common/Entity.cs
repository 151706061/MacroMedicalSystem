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

namespace Macro.Web.Common
{
	[DataContract(Namespace = Namespace.Value)]
    public abstract class Entity : IEquatable<Entity>
    {
        [DataMember(IsRequired = true)]
        public Guid Identifier { get; set; }

		public override string ToString()
		{
			return String.Format("{0}:{1}", this.GetType().Name, Identifier);
		}

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj is Entity)
                return Equals((Entity)obj);

            return false;
        }

        public static bool operator ==(Entity e1, Entity e2)
        {
            return Object.Equals(e1, e2);
        }

        public static bool operator !=(Entity e1, Entity e2)
        {
            return !Object.Equals(e1, e2);
        }

        public bool Equals(Entity other)
        {
            return other.Identifier == Identifier;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        #endregion

		public static T Create<T>() where T : Entity, new()
		{
			return new T { Identifier = Guid.NewGuid() };
		}
	}
}