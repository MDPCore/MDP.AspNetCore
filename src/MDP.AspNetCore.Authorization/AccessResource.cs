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

            if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentException($"{nameof(resourceUri)}=null");

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

                if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentException($"{nameof(resourceUri)}=null");

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
