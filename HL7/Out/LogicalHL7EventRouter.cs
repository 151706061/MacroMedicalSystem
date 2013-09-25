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
using ClearCanvas.Healthcare;

namespace ClearCanvas.HL7.Out
{
    public class LogicalHL7EventRouter<THandlerInterface, TExtensionPoint> : ILogicalHL7EventListener
        where THandlerInterface : class, ILogicalHL7EventHandler 
        where TExtensionPoint : ExtensionPoint<THandlerInterface>, new()
    {
        private readonly IDictionary<string, Type> _handlers;

        public LogicalHL7EventRouter()
        {
            _handlers = new Dictionary<string, Type>();
            foreach (THandlerInterface handler in new TExtensionPoint().CreateExtensions())
            {
                foreach (string supportedEvent in handler.GetSupportedLogicalEventTypes())
                {
                    _handlers.Add(supportedEvent, handler.GetType());
                }
            }
        }

        #region ILogicalHL7EventListener Members

        public void OnEvent(LogicalHL7EventArgs eventArgs)
        {
            if (_handlers.ContainsKey(eventArgs.EventType))
            {
                var instance = Activator.CreateInstance(_handlers[eventArgs.EventType]) as THandlerInterface;
                if (instance != null)
                    instance.HandleEvent(eventArgs);
            }
        }

        #endregion
    }
}