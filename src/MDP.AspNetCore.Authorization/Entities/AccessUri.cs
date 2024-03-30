using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class AccessUri
    {
        // Fields
        private readonly Uri _accessUri = null;

        private readonly string _accessString = null;

        private List<string> _accessPathList = null;


        // Constructors
        public AccessUri(string accessUri)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessUri) == true) throw new ArgumentException($"{nameof(accessUri)}=null");

            #endregion

            // AccessUri
            _accessUri = new Uri(accessUri);
            {
                // AccessUri.AccessProvider
                if (string.IsNullOrEmpty(this.AccessProvider) == true) throw new InvalidOperationException($"{nameof(this.AccessProvider)}=null");

                // AccessUri.AccessType
                if (string.IsNullOrEmpty(this.AccessType) == true) throw new InvalidOperationException($"{nameof(this.AccessType)}=null");

                // AccessUri.AccessPath
                if (this.AccessPath == "/") throw new InvalidOperationException($"{nameof(this.AccessPath)}=null");
                if (this.AccessPath == null) throw new InvalidOperationException($"{nameof(this.AccessPath)}=null");
                if (this.AccessPath == string.Empty) throw new InvalidOperationException($"{nameof(this.AccessPath)}=null");
            }

            // AccessString
            _accessString = $"{this.AccessType}://{this.AccessProvider}{this.AccessPath}".Replace("\\", "/");
        }


        // Properties
        public string AccessProvider { get { return _accessUri.Host; } }

        public string AccessType { get { return _accessUri.Scheme; } }

        public string AccessPath { get { return _accessUri.AbsolutePath; } }

        public string AccessString { get { return _accessString; } }

        internal List<string> AccessPathList
        {
            get
            {
                // Create
                if (_accessPathList == null)
                {
                    _accessPathList = this.AccessPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (_accessPathList == null) throw new InvalidOperationException($"{nameof(_accessPathList)}=null");

                // Return
                return _accessPathList;
            }
        }
    }
}
