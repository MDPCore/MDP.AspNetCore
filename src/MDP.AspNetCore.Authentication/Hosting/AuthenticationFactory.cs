using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication
{
    public class AuthenticationFactory : ServiceFactory<WebApplicationBuilder, AuthenticationFactory.Setting>
    {
        // Constructors
        public AuthenticationFactory() : base("Authentication", null, true) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentNullException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.DefaultScheme) == true) throw new InvalidOperationException("authenticationSetting.DefaultScheme=null");
            if (string.IsNullOrEmpty(setting.LoginPath) == true) throw new InvalidOperationException("authenticationSetting.LoginPath=null");
            if (string.IsNullOrEmpty(setting.LogoutPath) == true) throw new InvalidOperationException("authenticationSetting.LogoutPath=null");
            if (string.IsNullOrEmpty(setting.AccessDeniedPath) == true) throw new InvalidOperationException("authenticationSetting.AccessDeniedPath=null");

            // Authentication
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication(options =>
            {
                // Options
                options.DefaultScheme = setting.DefaultScheme;
                options.DefaultChallengeScheme = PolicyAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = PolicyAuthenticationDefaults.AuthenticationScheme;
            });
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // PolicyAuthentication
            authenticationBuilder.AddPolicy(new PolicyAuthenticationSetting()
            {
                // Options
                DefaultScheme = setting.DefaultScheme
            });

            // LocalAuthentication
            if (setting.DefaultScheme == LocalAuthenticationDefaults.AuthenticationScheme)
            {
                authenticationBuilder.AddLocal(options =>
                {
                    // Options
                    options.LoginPath = new PathString(setting.LoginPath);
                    options.LogoutPath = new PathString(setting.LogoutPath);
                    options.AccessDeniedPath = new PathString(setting.AccessDeniedPath);
                });
            }

            // RemoteAuthentication
            {
                authenticationBuilder.AddRemote(options =>
                {
                    // Options
                    options.ForwardChallenge = setting.DefaultScheme;
                    options.ForwardForbid = setting.DefaultScheme;
                });
            }

            // AuthenticationSetting
            applicationBuilder.Services.TryAddSingleton(serviceProvider =>
            {
                // Create
                var authenticationSetting = new AuthenticationSetting();
                authenticationSetting.RegisterPath = setting.RegisterPath;

                // Return
                return authenticationSetting;
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string DefaultScheme { get; set; } = LocalAuthenticationDefaults.AuthenticationScheme;

            public string LoginPath { get; set; } = "/Account/Login";

            public string LogoutPath { get; set; } = "/Account/Logout";

            public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";

            public string RegisterPath { get; set; } = null;
        }
    }
}