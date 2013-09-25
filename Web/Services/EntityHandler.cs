#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;
using Macro.Web.Common;
using Macro.Web.Common.Events;

namespace Macro.Web.Services
{
	//TODO: add extension point, much like desktop view extension points

	public interface IEntityHandler
	{
		Guid Identifier { get; }
		string Name { get; }

		Entity GetEntity();

		void SetModelObject(object modelObject);
		void ProcessMessage(Message message);
	}

	public abstract class EntityHandler<TEntity> : EntityHandler where TEntity : Entity, new()
	{
		protected EntityHandler()
		{
		}

		protected sealed override Entity CreateEntity()
		{
			return new TEntity();
		}

		protected sealed override void UpdateEntity(Entity entity)
		{
			UpdateEntity((TEntity)entity);
		}

		protected abstract void UpdateEntity(TEntity entity);

		public new TEntity GetEntity()
		{
			return (TEntity)base.GetEntity();
		}
	}

	public abstract class EntityHandler : IEntityHandler, IDisposable
	{
		#region Private Fields

		private bool _disposed;
		private string _name;

		#endregion
		
		protected EntityHandler()
		{
			Identifier = Guid.NewGuid();
		}

		public IApplicationContext ApplicationContext { get; set; }
		public virtual string Name
		{
			get
			{
				if (_name == null)
					_name = CreateEntity().GetType().Name;
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		#region IEntityHandler Members

		public Guid Identifier { get; set; }

		public virtual Entity GetEntity()
		{
			Entity entity = CreateEntity();
			UpdateEntity(entity);
			entity.Identifier = Identifier;
			return entity;
		}

		public abstract void SetModelObject(object modelObject);
		public abstract void ProcessMessage(Message message);
	
		protected abstract Entity CreateEntity();
		protected abstract void UpdateEntity(Entity entity);
        
        protected virtual string[] GetDebugInfo()
        {
            return null;
        }

		#endregion

        /// <summary>
        /// Sends a <see cref="PropertyChangedEvent"/> event to the client. The event may not be sent immediately.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected virtual void NotifyEntityPropertyChanged(string propertyName, object value)
		{
            Event @event = new PropertyChangedEvent
            {
				Identifier = Guid.NewGuid(),
				SenderId = Identifier,
				Sender = Name,
				PropertyName = propertyName,
                Value = value,
                DebugInfo = GetDebugInfo()
            };

            // Only fire the event if the entity handler has not been disposed.
			if (!_disposed)
                ApplicationContext.FireEvent(@event);
		}


	    protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			if (ApplicationContext != null)
				ApplicationContext.EntityHandlers.Remove(this);
		}

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				_disposed = true;
				Dispose(true);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#endregion

		public static THandler Create<THandler>()
			where THandler : EntityHandler, new()
		{
			EntityHandler handler = new THandler
			{
				ApplicationContext = Services.ApplicationContext.Current
			};

			if (handler.ApplicationContext == null)
				throw new InvalidOperationException("The Create method must be executed on the pseudo-UI thread");

			handler.ApplicationContext.EntityHandlers.Add(handler);
			return (THandler)handler;
		}
	}
}
