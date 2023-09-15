using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public class AuthenticationProvider
    {
        // Methods
        public virtual ClaimsIdentity Login(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // LocalIdentity
            var localIdentity = remoteIdentity;

            // Return
            return localIdentity;
        }

        public virtual void Link(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");
            if (localIdentity == null) throw new ArgumentException($"{nameof(localIdentity)}=null");

            #endregion

            // Nothing
           
        }
    }
}
