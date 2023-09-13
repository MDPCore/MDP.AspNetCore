using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class RemoteAuthenticationOptionsExtensions
    {
        // Methods
        public static void SignInPath(this RemoteAuthenticationOptions remoteAuthenticationOptions, string signInPath = "/.auth/signin")
        {
            #region Contracts

            if (string.IsNullOrEmpty(signInPath) == true) throw new ArgumentException(nameof(signInPath));
         
            #endregion

            // OnTicketReceived
            remoteAuthenticationOptions.Events.OnTicketReceived = context =>
            {
                // ReturnUrl
                var returnUrl = new PathString(signInPath).Add(QueryString.Create(new Dictionary<string, string>()
                {
                    { "returnUrl", context.ReturnUri! }
                }!));
                if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");

                // Setting
                context.ReturnUri = returnUrl;

                // Return
                return Task.CompletedTask;
            };
        }
    }
}
