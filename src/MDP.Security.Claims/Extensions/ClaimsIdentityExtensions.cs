using System;
using System.Security.Claims;
using System.Security.Principal;

namespace System.Security.Claims
{
    public static class ClaimsIdentityExtensions
    {
        // Methods
        public static string GetClaimValue(this ClaimsPrincipal principal, string claimType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(claimType) == true) throw new ArgumentException(nameof(claimType));

            #endregion

            // Return
            return principal.Identity?.GetClaimValue(claimType);
        }

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