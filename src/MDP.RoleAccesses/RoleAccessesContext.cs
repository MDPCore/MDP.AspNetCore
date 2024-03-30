using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    [MDP.Registration.Service<RoleAccessesContext>(singleton: true)]
    public class RoleAccessesContext
    {
        // Fields
        private readonly PermissionRepository _permissionRepository = null;

        private readonly AccessRecordRepository _accessRecordRepository = null;


        // Constructors
        public RoleAccessesContext(PermissionRepository permissionRepository, AccessRecordRepository accessRecordRepository)
        {
            #region Contracts

            if (permissionRepository == null) throw new ArgumentException($"{nameof(permissionRepository)}=null");
            if (accessRecordRepository == null) throw new ArgumentException($"{nameof(accessRecordRepository)}=null");

            #endregion

            // Default
            _permissionRepository = permissionRepository;
            _accessRecordRepository = accessRecordRepository;
        }


        // Methods
        public bool HasAccess(List<string> roleIdList, string resourceUri)
        {
            #region Contracts

            if (roleIdList == null) throw new ArgumentException($"{nameof(roleIdList)}=null");
            if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // Return
            return this.HasAccess(roleIdList, new ResourceUri(resourceUri));
        }

        public bool HasAccess(List<string> roleIdList, ResourceUri resourceUri)
        {
            #region Contracts

            if (roleIdList == null) throw new ArgumentException($"{nameof(roleIdList)}=null");
            if (resourceUri == null) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // HasAccess
            foreach (var roleId in roleIdList)
            {
                if (this.HasAccess(roleId, resourceUri) == true)
                {
                    return true;
                }
            }

            // Return
            return false;
        }


        public bool HasAccess(string roleId, string resourceUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // Return
            return this.HasAccess(roleId, new ResourceUri(resourceUri));
        }

        public bool HasAccess(string roleId, ResourceUri resourceUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (resourceUri == null) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // Result
            var accessResult = false;

            // AccessRecord
            var accessRecord = _accessRecordRepository.FindByRoleId(roleId, resourceUri);
            if (accessRecord != null) return accessRecord.AccessResult;

            // PermissionList
            var permissionList = _permissionRepository.FindAllByRoleId(roleId, resourceUri.ResourceProvider, resourceUri.ResourceType);
            if (permissionList == null) throw new InvalidOperationException($"{nameof(permissionList)}=null");

            // HasAccess
            foreach (var permission in permissionList)
            {
                if (permission.HasAccess(roleId, resourceUri) == true)
                {
                    accessResult = true;
                    break;
                }
            }

            // Return
            accessRecord = new AccessRecord(roleId, resourceUri, accessResult);
            {
                _accessRecordRepository.Add(accessRecord);
            }
            return accessResult;
        }
    }
}
