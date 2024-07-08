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
        public virtual ClaimsIdentity LocalLogin(ClaimsIdentity localIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(localIdentity);

            #endregion

            // Return
            return localIdentity;
        }

        public virtual ClaimsIdentity RemoteLogin(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(remoteIdentity);

            #endregion

            // Return
            return remoteIdentity;
        }

        public virtual void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(remoteIdentity);
            ArgumentNullException.ThrowIfNull(localIdentity);

            #endregion

            // Nothing
           
        }
    }
}
