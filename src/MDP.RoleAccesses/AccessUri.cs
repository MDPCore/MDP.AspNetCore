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
    public class AccessUri
    {
        // Fields
        private readonly Uri _accessUri = null;

        private readonly string _accessString = null;


        // Constructors
        public AccessUri(string accessString)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessString) == true) throw new ArgumentException($"{nameof(accessString)}=null");

            #endregion

            // AccessUri
            _accessUri = new Uri(accessString);
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
            _accessString = Path.Combine(this.AccessProvider, this.AccessType, this.AccessPath).Replace("\\", "/");
        }


        // Properties
        public string AccessProvider { get { return _accessUri.Host; } }

        public string AccessType { get { return _accessUri.Scheme; } }

        public string AccessPath { get { return _accessUri.AbsolutePath; } }

        public string AccessString { get { return _accessString; } }
    }
}
