#region License

//HL7 Support for Clear Canvas RIS
//Copyright (C)  2012 Archibald Archibaldovitch

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.HL7.In
{
    public class HL7MessageRouter<THandlerInterface, TExtensionPoint> : IIncomingHL7MessageListener
        where THandlerInterface : class, IHL7MessageHandler
        where TExtensionPoint : ExtensionPoint<THandlerInterface>, new()
    {
        private readonly IDictionary<string, Type> _handlers;

        public HL7MessageRouter()
        {
            _handlers = new Dictionary<string, Type>();
            foreach (THandlerInterface handler in new TExtensionPoint().CreateExtensions())
            {
                foreach (var supportedMessage in handler.GetHandledMessageTypes())
                {
                    _handlers.Add(supportedMessage, handler.GetType());
                }
            }
        }

        #region IIncomingHL7MessageListener Members

        public void OnMessage(HL7Message message)
        {
            if (_handlers.ContainsKey(message.Type))
            {
                var instance = Activator.CreateInstance(_handlers[message.Type]) as THandlerInterface;
                if (instance != null)
                    instance.Handle(message);
            }
        }

        #endregion
    }
}