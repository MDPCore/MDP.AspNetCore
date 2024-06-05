using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore
{
    public static class ProblemDetailsMiddlewareExtensions
    {
        // Methods 
        public static WebApplication UseProblemDetails(this WebApplication host)
        {
            #region Contracts

            if (host == null) throw new ArgumentNullException($"{nameof(host)}=null");

            #endregion

            // ProblemDetails
            ProblemDetailsExtensions.UseProblemDetails(host);

            // Return
            return host;
        }
    }
}
