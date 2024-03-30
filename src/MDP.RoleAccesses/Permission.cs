using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MDP.RoleAccesses;

namespace MDP.RoleAccesses
{
    public class Permission
    {
        // Constants
        private static readonly string _doubleAsteriskString = Guid.NewGuid().ToString();


        // Fields
        private Regex _accessPattern = null;


        // Constructors
        public Permission(string permissionId, string roleId, string accessUri) : this(permissionId, roleId, new AccessUri(accessUri))
        {
            
        }

        public Permission(string permissionId, string roleId, AccessUri accessUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(permissionId) == true) throw new ArgumentException($"{nameof(permissionId)}=null");
            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (accessUri == null) throw new ArgumentException($"{nameof(accessUri)}=null");

            #endregion

            // Default
            this.PermissionId = permissionId;
            this.RoleId = roleId;
            this.AccessUri = accessUri;

            // Create
            _accessPattern = CreateAccessPattern(accessUri.AccessString);
        }


        // Properties
        public string PermissionId { get; set; }

        public string RoleId { get; set; }

        public AccessUri AccessUri { get; set; }

        private Regex AccessPattern
        {
            get
            {
                // Create
                if (_accessPattern == null)
                {
                    _accessPattern = this.CreateAccessPattern(this.AccessUri.AccessString);
                }
                if (_accessPattern == null) throw new InvalidOperationException($"{nameof(_accessPattern)}=null");

                // Return
                return _accessPattern;
            }
        }


        // Methods
        public bool HasAccess(string roleId, ResourceUri resourceUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (resourceUri == null) throw new ArgumentException($"{nameof(resourceUri)}=null");
            
            #endregion

            // RoleId
            if (this.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == false) return false;

            // ResourceUri
            if (this.AccessPattern.IsMatch(resourceUri.ResourceString) == false) return false;

            // Return
            return true;
        }

        private Regex CreateAccessPattern(string accessString)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessString) == true) throw new ArgumentException($"{nameof(accessString)}=null");

            #endregion

            // AccessPatternString
            var accessPatternString = accessString;
            accessPatternString = accessPatternString.Replace("**", _doubleAsteriskString);
            accessPatternString = accessPatternString.Replace("*", "[^/]*");
            accessPatternString = accessPatternString.Replace(_doubleAsteriskString, ".*");
            accessPatternString = "^" + accessPatternString + "$";

            // AccessPattern
            var accessPattern = new Regex(accessPatternString, RegexOptions.IgnoreCase);

            // Return
            return accessPattern;
        }
    }
}
