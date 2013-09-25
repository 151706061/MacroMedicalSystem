#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ClearCanvas.Web.Common
{


    [DataContract(Namespace = Namespace.Value)]
    public class Command
    {
        [DataMember(IsRequired = true)]
        public Guid Identifier { get; set; }

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj is Command)
                return Equals((Command)obj);

            return false;
        }

        public static bool operator ==(Command e1, Command e2)
        {
            return Object.Equals(e1, e2);
        }

        public static bool operator !=(Command e1, Command e2)
        {
            return !Object.Equals(e1, e2);
        }

        #region IEquatable<Command> Members

        public bool Equals(Command other)
        {
            return other.Identifier == Identifier;
        }

        #endregion
        #endregion
    }


    [DataContract(Namespace = Namespace.Value)]
    public class CommandSet
    {
        [DataMember(IsRequired = true)]
        public Command[] Commands { get; set; }
    }
}