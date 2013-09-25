using System.Collections.Generic;
using System.Security.Principal;
using Macro.Enterprise.Core;
using Macro.ImageServer.Enterprise;
using Macro.ImageServer.Model;
using Macro.ImageServer.Model.EntityBrokers;
using Macro.Web.Enterprise.Authentication;

namespace Macro.ImageServer.Web.Common.Utilities
{
    public static class DataAccessHelper
    {
        private const string DataAccessSubCriteriaPrefix = "DataAccessSubCriteria";

        private static string GetDataAccessSubCriteriaCacheID(CustomPrincipal principal)
        {
            return DataAccessSubCriteriaPrefix + principal.SessionTokenId;
        }

        public static StudyDataAccessSelectCriteria GetDataAccessSubCriteriaForUser(IPersistenceContext context, IPrincipal user)
        {
            if (user.IsInRole(Macro.Enterprise.Common.AuthorityTokens.DataAccess.AllStudies))
            {
                return null;
            }

            var principal = user as CustomPrincipal;
            if (principal == null)
                return null;

            string key = GetDataAccessSubCriteriaCacheID(principal);

            // check the cache first
            var subCriteria = Cache.Current[key] as StudyDataAccessSelectCriteria;
            if (subCriteria != null)
                return subCriteria;


            var oidList = new List<ServerEntityKey>();
            foreach (var oid in principal.Credentials.DataAccessAuthorityGroups)
                oidList.Add(new ServerEntityKey("OID", oid));
            var dataAccessGroupSelectCriteria = new DataAccessGroupSelectCriteria();
            dataAccessGroupSelectCriteria.AuthorityGroupOID.In(oidList);

            IList<DataAccessGroup> groups;
            var broker = context.GetBroker<IDataAccessGroupEntityBroker>();
            groups = broker.Find(dataAccessGroupSelectCriteria);


            var entityList = new List<ServerEntityKey>();
            foreach (DataAccessGroup group in groups)
            {
                entityList.Add(group.Key);
            }

            subCriteria = new StudyDataAccessSelectCriteria();
            subCriteria.DataAccessGroupKey.In(entityList);

            // put into cache for re-use
            Cache.Current[key] = subCriteria;

            return subCriteria;
        }

    }
}