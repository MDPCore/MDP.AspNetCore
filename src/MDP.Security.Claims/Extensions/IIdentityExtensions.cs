using System;
using System.Security.Claims;
using System.Security.Principal;

namespace MDP.Security.Claims
{
    public static class IIdentityExtensions
    {
        // Methods   
        public static string GetClaimValue(this IIdentity identity, string claimType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(claimType) == true) throw new ArgumentException(nameof(claimType));

            #endregion

            // Return
            return (identity as ClaimsIdentity)?.FindFirst(claimType)?.Value;
        }
    }
}