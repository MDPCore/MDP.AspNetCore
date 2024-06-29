﻿using System;
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
        public virtual ClaimsIdentity RemoteLogin(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentNullException($"{nameof(remoteIdentity)}=null");

            #endregion

            // LocalIdentity
            var localIdentity = remoteIdentity;

            // Return
            return localIdentity;
        }

        public virtual void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentNullException($"{nameof(remoteIdentity)}=null");
            if (localIdentity == null) throw new ArgumentNullException($"{nameof(localIdentity)}=null");

            #endregion

            // Nothing
           
        }

        public virtual ClaimsIdentity LocalRefresh(ClaimsIdentity localIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(localIdentity);

            #endregion

            // freshIdentity
            var freshIdentity = localIdentity;

            // Return
            return freshIdentity;
        }
    }
}