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
        public AuthenticationFactory() : base("Authentication") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

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
                options.DefaultAuthenticateScheme = PolicyAuthenticationDefaults.AuthenticationScheme;
            });

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

            // AuthenticationControllerSetting
            applicationBuilder.Services.TryAddSingleton(serviceProvider =>
            {
                // Create
                var authenticationControllerSetting = new AuthenticationControllerSetting();
                authenticationControllerSetting.RegisterPath = setting.RegisterPath;

                // Return
                return authenticationControllerSetting;
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