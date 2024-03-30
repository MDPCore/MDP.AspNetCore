using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class Resource
    {
        // Fields
        private readonly ResourceUri _resourceUri = null;


        // Consructors
        public Resource(string resourceUri)
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
    }
}
