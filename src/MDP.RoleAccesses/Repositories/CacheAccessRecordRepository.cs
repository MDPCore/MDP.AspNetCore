using CLK.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    [MDP.Registration.Service<AccessRecordRepository>()]
    public class CacheAccessRecordRepository : AccessRecordRepository
    {
        // Fields
        private readonly ExpirationMemoryCache _accessRecordCache = new ExpirationMemoryCache();


        // Methods
        public void Add(AccessRecord accessRecord)
        {
            #region Contracts

            if (accessRecord == null) throw new ArgumentException($"{nameof(accessRecord)}=null");

            #endregion

            // CacheKey
            var cacheKey = $"{accessRecord.RoleId}/{accessRecord.ResourceUri.ResourceString}";
            if (string.IsNullOrEmpty(cacheKey) == true) throw new InvalidOperationException($"{nameof(cacheKey)}=null");

            // SetValue
            _accessRecordCache.SetValue(cacheKey, accessRecord);
        }

        public AccessRecord FindByRoleId(string roleId, ResourceUri resourceUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (resourceUri == null) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // CacheKey
            var cacheKey = $"{roleId}/{resourceUri.ResourceString}";
            if (string.IsNullOrEmpty(cacheKey) == true) throw new InvalidOperationException($"{nameof(cacheKey)}=null");

            // TryGetValue
            AccessRecord accessRecord = null;
            if(_accessRecordCache.TryGetValue(cacheKey, out accessRecord)==true)
            {
                return accessRecord;
            }

            // Return
            return null;
        }
    }
}
