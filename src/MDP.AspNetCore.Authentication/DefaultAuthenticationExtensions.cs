using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class DefaultAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddDefaultAuthentication(this IServiceCollection services, DefaultAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new DefaultAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.DefaultScheme) == true) throw new InvalidOperationException("authenticationSetting.DefaultScheme=null");
            if (string.IsNullOrEmpty(authenticationSetting.LoginPath) == true) throw new InvalidOperationException("authenticationSetting.LoginPath=null");
            if (string.IsNullOrEmpty(authenticationSetting.LogoutPath) == true) throw new InvalidOperationException("authenticationSetting.LogoutPath=null");
            if (string.IsNullOrEmpty(authenticationSetting.AccessDeniedPath) == true) throw new InvalidOperationException("authenticationSetting.AccessDeniedPath=null");
            services.TryAddSingleton(authenticationSetting);

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                // DefaultScheme
                options.DefaultScheme = authenticationSetting.DefaultScheme;
                options.DefaultAuthenticateScheme = PolicyAuthenticationDefaults.AuthenticationScheme;
            });

            // Policy
            authenticationBuilder.AddPolicy(new PolicyAuthenticationSetting()
            {
                // DefaultScheme
                DefaultScheme = authenticationSetting.DefaultScheme
            });

            // Local
            if (authenticationSetting.DefaultScheme == LocalAuthenticationDefaults.AuthenticationScheme)
            {
                authenticationBuilder.AddLocal(options =>
                {
                    // Options
                    options.LoginPath = new PathString(authenticationSetting.LoginPath);
                    options.LogoutPath = new PathString(authenticationSetting.LogoutPath);
                    options.AccessDeniedPath = new PathString(authenticationSetting.AccessDeniedPath);
                });
            }

            // Remote
            {
                authenticationBuilder.AddRemote(options =>
                {
                    // Options
                    options.ForwardChallenge = authenticationSetting.DefaultScheme;
                    options.ForwardForbid = authenticationSetting.DefaultScheme;
                });
            }

            // Return
            return authenticationBuilder;
        }
    }
}
