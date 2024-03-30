using CLK.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    [MDP.Registration.Service<PermissionRepository>()]
    public class CachePermissionRepository : PermissionRepository
    {
        // Fields
        private readonly ExpirationMemoryCache _permissionListCache = new ExpirationMemoryCache();

        private readonly PermissionRepository _permissionRepository = null;


        // Constructors
        public CachePermissionRepository(PermissionRepository permissionRepository)
        {
            #region Contracts

            if (permissionRepository == null) throw new ArgumentException($"{nameof(permissionRepository)}=null");

            #endregion

            // Default
            _permissionRepository = permissionRepository;
        }


        // Methods
        public List<Permission> FindAllByRoleId(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // CacheKey
            var cacheKey = $"{roleId}/{accessProvider}/{accessType}";
            if (string.IsNullOrEmpty(cacheKey) == true) throw new InvalidOperationException($"{nameof(cacheKey)}=null");

            // PermissionList
            var permissionList = _permissionListCache.GetValue(cacheKey, () => 
            {
                // PermissionRepository
                return _permissionRepository.FindAllByRoleId(roleId, accessProvider, accessType);
            });
            if (permissionList == null) throw new InvalidOperationException($"{nameof(permissionList)}=null");

            // FindAll
            return permissionList;
        }
    }
}
