using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class AccessResource
    {
        // Fields
        private readonly ResourceUri _resourceUri = null;


        // Consructors
        public AccessResource(string resourceUri)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNullOrEmpty(resourceUri);

            #endregion

            // ResourceUri
            _resourceUri = new ResourceUri(resourceUri);
        }


        // Properties
        public string ResourceProvider { get { return _resourceUri.ResourceProvider; } }

        public string ResourceType { get { return _resourceUri.ResourceType; } }

        public string ResourcePath { get { return _resourceUri.ResourcePath; } }

        public string ResourceString { get { return _resourceUri.ResourceString; } }

        internal List<string> ResourcePathList { get { return _resourceUri.ResourcePathList; } }


        // Methods
        public bool HasAccess(RoleAssignment roleAssignment, AccessPermission accessPermission)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(roleAssignment);
            ArgumentNullException.ThrowIfNull(accessPermission);

            #endregion

            // Require
            if (roleAssignment.RoleId.Equals(accessPermission.RoleId, StringComparison.OrdinalIgnoreCase) == false) throw new InvalidOperationException($"{nameof(roleAssignment.RoleId)}!=${nameof(accessPermission.RoleId)}");
            if (roleAssignment.RoleScopes.Count != accessPermission.RoleScopes.Count) throw new InvalidOperationException($"{nameof(roleAssignment.RoleScopes)}!=${nameof(accessPermission.RoleScopes)}");

            // ResourceString
            if (this.ResourceString.Equals(accessPermission.AccessString, StringComparison.OrdinalIgnoreCase) == true) return true;

            // ResourceProvider
            if (this.ResourceProvider.Equals(accessPermission.AccessProvider, StringComparison.OrdinalIgnoreCase) == false) return false;

            // ResourceType
            if (this.ResourceType.Equals(accessPermission.AccessType, StringComparison.OrdinalIgnoreCase) == false) return false;

            // ResourcePath.Count
            var maxCount = Math.Max(this.ResourcePathList.Count, accessPermission.AccessPathList.Count);
            if (maxCount == 0) return true;

            // ResourcePath.Equals
            for (int i = 0; i < maxCount; i++)
            {
                // Variables
                var resourcePathSection = this.ResourcePathList.ElementAtOrDefault(i);
                var accessPathSection = accessPermission.AccessPathList.ElementAtOrDefault(i);

                // Null
                if (string.IsNullOrEmpty(resourcePathSection) == true) return false;
                if (string.IsNullOrEmpty(accessPathSection) == true) return false;

                // [Scope]
                if (accessPathSection.StartsWith("[") == true && accessPathSection.EndsWith("]") == true)
                {
                    // ResourceScope
                    var resourceScope = resourcePathSection;
                    if (string.IsNullOrEmpty(resourceScope) == true) return false;

                    // AccessScopeKey
                    var accessScopeKey = accessPathSection.Substring(1, accessPathSection.Length - 2);
                    if (string.IsNullOrEmpty(accessScopeKey) == true) return false;

                    // RoleScopeIndex
                    var roleScopeIndex = accessPermission.RoleScopes.FindIndex(o => o.Equals(accessScopeKey, StringComparison.OrdinalIgnoreCase) == true);
                    if (roleScopeIndex <= -1) return false;                   

                    // RoleScope
                    var roleScope = roleAssignment.RoleScopes.ElementAtOrDefault(roleScopeIndex);
                    if (string.IsNullOrEmpty(roleScope) == true) return false;

                    // ResourceScope.Equals
                    if (resourceScope.Equals(roleScope, StringComparison.OrdinalIgnoreCase) == true)
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
                    // Last
                    if (i == accessPermission.AccessPathList.Count - 1) break;

                    // Middle
                    if (i <  accessPermission.AccessPathList.Count - 1) continue;                    
                }

                // String
                if (resourcePathSection.Equals(accessPathSection, StringComparison.OrdinalIgnoreCase) == false) return false;
            }

            // Return
            return true;
        }


        // Class
        private class ResourceUri
        {
            // Fields
            private readonly Uri _resourceUri = null;

            private readonly string _resourceString = null;

            private List<string> _resourcePathList = null;


            // Constructors
            public ResourceUri(string resourceUri)
            {
                #region Contracts

                if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentNullException($"{nameof(resourceUri)}=null");

                #endregion

                // ResourceUri
                _resourceUri = new Uri(resourceUri);
                {
                    // ResourceUri.ResourceProvider
                    if (string.IsNullOrEmpty(this.ResourceProvider) == true) throw new InvalidOperationException($"{nameof(this.ResourceProvider)}=null");

                    // ResourceUri.ResourceType
                    if (string.IsNullOrEmpty(this.ResourceType) == true) throw new InvalidOperationException($"{nameof(this.ResourceType)}=null");

                    // ResourceUri.ResourcePath
                    if (this.ResourcePath == "/") throw new InvalidOperationException($"{nameof(this.ResourcePath)}=null");
                    if (this.ResourcePath == null) throw new InvalidOperationException($"{nameof(this.ResourcePath)}=null");
                    if (this.ResourcePath == string.Empty) throw new InvalidOperationException($"{nameof(this.ResourcePath)}=null");
                }

                // ResourceString
                _resourceString = $"{this.ResourceType}://{this.ResourceProvider}{this.ResourcePath}".Replace("\\", "/");
            }


            // Properties
            public string ResourceProvider { get { return _resourceUri.Host; } }

            public string ResourceType { get { return _resourceUri.Scheme; } }

            public string ResourcePath { get { return _resourceUri.AbsolutePath; } }

            public string ResourceString { get { return _resourceString; } }

            internal List<string> ResourcePathList
            {
                get
                {
                    // Create
                    if (_resourcePathList == null)
                    {
                        _resourcePathList = this.ResourcePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    if (_resourcePathList == null) throw new InvalidOperationException($"{nameof(_resourcePathList)}=null");

                    // Return
                    return _resourcePathList;
                }
            }
        }
    }
}
