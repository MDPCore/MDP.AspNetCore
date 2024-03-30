using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    public class ResourceUri
    {
        // Fields
        private readonly Uri _resourceUri = null;

        private readonly string _resourceString = null;


        // Constructors
        public ResourceUri(string resourceString)
        {
            #region Contracts

            if (string.IsNullOrEmpty(resourceString) == true) throw new ArgumentException($"{nameof(resourceString)}=null");

            #endregion

            // ResourceUri
            _resourceUri = new Uri(resourceString);
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
            _resourceString = Path.Combine(this.ResourceProvider, this.ResourceType, this.ResourcePath).Replace("\\", "/");
        }


        // Properties
        public string ResourceProvider { get { return _resourceUri.Host; } }

        public string ResourceType { get { return _resourceUri.Scheme; } }

        public string ResourcePath { get { return _resourceUri.AbsolutePath; } }

        public string ResourceString { get { return _resourceString; } }
    }
}
