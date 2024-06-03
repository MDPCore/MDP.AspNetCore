using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class HttpContextExtensions
    {
        // Methods
        public static async Task LoginAsync(this HttpContext httpContext, ClaimsIdentity localIdentity)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(httpContext);
            ArgumentNullException.ThrowIfNull(localIdentity);

            #endregion

            // Sign
            await httpContext.RemoteSignOutAsync();
            await httpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));
        }

        public static async Task LogoutAsync(this HttpContext httpContext)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(httpContext);

            #endregion

            // Sign
            await httpContext.RemoteSignOutAsync();
            await httpContext.LocalSignOutAsync();
        }
    }
}