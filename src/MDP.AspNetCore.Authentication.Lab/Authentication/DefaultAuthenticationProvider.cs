using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Linq;

namespace MDP.AspNetCore.Authentication.Lab
{
    [MDP.Registration.Service<AuthenticationProvider>(singleton: true)]
    public class DefaultAuthenticationProvider : AuthenticationProvider
    {
        // Methods
        public override ClaimsIdentity LocalLogin(ClaimsIdentity localIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(localIdentity);

            #endregion

            // Return
            return this.Login(localIdentity);
        }

        public override ClaimsIdentity RemoteLogin(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(remoteIdentity);

            #endregion

            // Return
            return this.Login(remoteIdentity);
        }

        private ClaimsIdentity Login(ClaimsIdentity identity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(identity);

            #endregion

            // NowTime
            var dateTimeString = DateTime.Now.ToString("HH:mm:ss.fff");

            // ClaimList
            var claimList = identity.Claims.ToList();
            claimList.RemoveAll(o => o.Type == ClaimTypes.Role && o.Value.StartsWith("Role")== true);
            claimList.Add(new Claim(ClaimTypes.Role, $"Role{dateTimeString}"));

            // Return
            return new ClaimsIdentity(claimList, identity.AuthenticationType);
        }
    }
}
