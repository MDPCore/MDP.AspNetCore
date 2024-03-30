using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class Permission
    {
        // Fields
        private readonly AccessUri _accessUri = null;


        // Consructors
        public Permission(string roleId, string accessUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessUri) == true) throw new ArgumentException($"{nameof(accessUri)}=null");

            #endregion

            // Default
            this.RoleId = roleId;

            // AccessUri
            _accessUri = new AccessUri(accessUri);
        }


        // Properties
        public string RoleId { get; set; }

        public string AccessProvider { get { return _accessUri.AccessProvider; } }

        public string AccessType { get { return _accessUri.AccessType; } }

        public string AccessPath { get { return _accessUri.AccessPath; } }

        public string AccessString { get { return _accessUri.AccessString; } }

        internal List<string> AccessPathList { get { return _accessUri.AccessPathList; } }


        // Methods
        public bool HasAccess(RoleAssignment roleAssignment, Resource resource)
        {
            #region Contracts

            if (roleAssignment == null) throw new ArgumentException($"{nameof(roleAssignment)}=null");
            if (resource == null) throw new ArgumentException($"{nameof(resource)}=null");

            #endregion

            // RoleId
            if (this.RoleId.Equals(roleAssignment.RoleId, StringComparison.OrdinalIgnoreCase) == false) return false;

            // AccessString
            if (this.AccessString.Equals(resource.ResourceString, StringComparison.OrdinalIgnoreCase) == true) return true;

            // AccessProvider
            if (this.AccessProvider.Equals(resource.ResourceProvider, StringComparison.OrdinalIgnoreCase) == false) return false;

            // AccessType
            if (this.AccessType.Equals(resource.ResourceType, StringComparison.OrdinalIgnoreCase) == false) return false;

            // AccessPath.Count
            var maxCount = Math.Max(this.AccessPathList.Count, resource.ResourcePathList.Count);
            if (maxCount == 0) return true;

            // AccessPath.Equals
            for (int i = 0; i < maxCount; i++)
            {
                // Variables
                var accessPathSection = this.AccessPathList.ElementAtOrDefault(i);
                var resourcePathSection = resource.ResourcePathList.ElementAtOrDefault(i);

                // Null
                if (string.IsNullOrEmpty(accessPathSection) == true) return false;
                if (string.IsNullOrEmpty(resourcePathSection) == true) return false;

                // [Scope]
                if (accessPathSection.StartsWith("[") == true && accessPathSection.EndsWith("]") == true) 
                {
                    // AccessScopeKey
                    var accessScopeKey = accessPathSection.Substring(1, accessPathSection.Length - 2);
                    if (string.IsNullOrEmpty(accessScopeKey) == true) return false;

                    // AccessScope
                    var accessScope = string.Empty;
                    if (roleAssignment.Scopes.TryGetValue(accessScopeKey, out accessScope) == false) return false;
                    if (string.IsNullOrEmpty(accessScope) == true) return false;

                    // ResourceScope
                    var resourceScope = resourcePathSection;
                    if (string.IsNullOrEmpty(resourceScope) == true) return false;

                    // Equals
                    if (accessScope.Equals(resourceScope, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }

                // *
                if (accessPathSection == "*")
                {
                    // Middle
                    if (i < this.AccessPathList.Count - 1) continue;

                    // Last
                    if (i == this.AccessPathList.Count - 1) return true;
                }

                // String
                if (accessPathSection.Equals(resourcePathSection, StringComparison.OrdinalIgnoreCase) == false) return false;
            }

            // Return
            return true;
        }
    }
}
