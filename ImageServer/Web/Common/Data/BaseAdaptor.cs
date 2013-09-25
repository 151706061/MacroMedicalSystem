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
using Macro.Common;
using Macro.Enterprise.Core;
using Macro.ImageServer.Enterprise;

namespace Macro.ImageServer.Web.Common.Data
{
    public class BaseAdaptor<TServerEntity, TIEntity, TCriteria, TColumns>
        where TServerEntity : ServerEntity, new()
        where TCriteria : EntitySelectCriteria, new()
        where TColumns : EntityUpdateColumns
        where TIEntity : IEntityBroker<TServerEntity, TCriteria, TColumns>
    {
        #region Private Members

        private readonly IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

        #endregion Private Members

        #region Protected Properties

        protected IPersistentStore PersistentStore
        {
            get { return _store; }
        }

        #endregion


        protected virtual void OnQuerying(IPersistenceContext context, TCriteria criteria)
        {
            
        }

        #region Public Methods

        public IList<TServerEntity> Get()
        {
            return Get(HttpContextData.Current.ReadContext);
        }
		public IList<TServerEntity> Get(IPersistenceContext context)
		{
		    TIEntity find = context.GetBroker<TIEntity>();
			TCriteria criteria = new TCriteria();

            OnQuerying(context, criteria);
			
            IList<TServerEntity> list = find.Find(criteria);

			return list;		
		}


        public TServerEntity Get(ServerEntityKey key)
		{
            return Get(HttpContextData.Current.ReadContext, key);
		}

		public TServerEntity Get(IPersistenceContext context, ServerEntityKey key)
		{
            //TODO: Add data access filter ?
		    TIEntity select = context.GetBroker<TIEntity>();
			return select.Load(key);
		}

    	public IList<TServerEntity> Get(TCriteria criteria)
        {
            return Get(HttpContextData.Current.ReadContext, criteria);
        }

		public IList<TServerEntity> Get(IPersistenceContext context, TCriteria criteria)
		{
            OnQuerying(context, criteria);
			TIEntity select = context.GetBroker<TIEntity>();
				return select.Find(criteria);

		}
		public IList<TServerEntity> GetRange(TCriteria criteria, int startIndex, int maxRows)
		{
            return GetRange(HttpContextData.Current.ReadContext, criteria, startIndex, maxRows);
		}
		public IList<TServerEntity> GetRange(IPersistenceContext context, TCriteria criteria, int startIndex, int maxRows)
		{
		    OnQuerying(context, criteria);
			TIEntity select = context.GetBroker<TIEntity>();

            // SQL row index starts from 1
		    int fromRowIndex = startIndex + 1;
            return select.Find(criteria, fromRowIndex, maxRows);
		}

    	public int GetCount(TCriteria criteria)
		{
    	    OnQuerying(HttpContextData.Current.ReadContext, criteria);
            TIEntity select = HttpContextData.Current.ReadContext.GetBroker<TIEntity>();
			return select.Count(criteria);
		}

		public TServerEntity GetFirst(TCriteria criteria)
		{
            return GetFirst(HttpContextData.Current.ReadContext, criteria);
		}

		public TServerEntity GetFirst(IPersistenceContext context, TCriteria criteria)
		{
            OnQuerying(context, criteria);
            TIEntity select = context.GetBroker<TIEntity>();
			return select.FindOne(criteria);
		}

        public TServerEntity Add(TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TServerEntity entity =  Add(context, param);
                    context.Commit();
                    return entity;
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception adding {0}", typeof (TServerEntity));
                throw;
            }
        }

        public TServerEntity Add(IUpdateContext context, TColumns param)
        {
        	TIEntity update = context.GetBroker<TIEntity>();

            TServerEntity newEntity = update.Insert(param);

            return newEntity;
        }


        public bool Update(ServerEntityKey key, TColumns param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    update.Update(key, param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                throw;
            }
        }

		public bool Delete(IUpdateContext context, ServerEntityKey key)
		{
			TIEntity update = context.GetBroker<TIEntity>();

			return update.Delete(key);
		}

    	public bool Delete(ServerEntityKey key)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    if (!update.Delete(key))
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof (TServerEntity));
                throw;
            }
        }

		public bool Delete(IUpdateContext context, TCriteria criteria)
		{
			TIEntity update = context.GetBroker<TIEntity>();

			if (update.Delete(criteria) < 0)
				return false;

			return true;
		}

    	public bool Delete(TCriteria criteria)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIEntity update = context.GetBroker<TIEntity>();

                    if (update.Delete(criteria) < 0)
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof(TServerEntity));
                throw;
            }
        }

        #endregion
    }
}
