#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Web.Common
{
    public interface IApplicationServiceContext
    {
        //dynamically add/remove command handlers?
        void AddCommandHandler(ICommandHandler handler);
        void RemoveCommandHandler(Guid identifier);

        //dynamically add/remove message handlers?
        void AddMessageHandler(IMessageHandler handler);
        void RemoveMessageHandler(Guid identifier);

        bool TryGetEntity(Guid theGuid, out object theValue);
        void AddEntity(Guid theGuid, object val);
        void RemoveEntity(Guid theGuid);

		void FatalError(Exception e);
		void FireEvent(Event @event);
		void ExecuteCommands(CommandSet commands);
		void ProcessMessages(MessageSet messages);
	}
}