using MDP.RoleAccesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    public class AccessRecord
    {
        // Constructors
        public AccessRecord(string roleId, ResourceUri resourceUri, bool accessResult)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (resourceUri == null) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // Default
            this.RoleId = roleId;
            this.ResourceUri = resourceUri;
            this.AccessResult = accessResult;
        }


        // Properties
        public string RoleId { get; set; }

        public ResourceUri ResourceUri { get; set; }

        public bool AccessResult { get; set; }
    }
}
