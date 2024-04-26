using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore
{
    public static partial class HookMiddlewareExtensions
    {
        // Methods        
        public static WebApplicationBuilder AddHook(this WebApplicationBuilder hostBuilder, string hookName, Action<WebApplication> configureMiddleware)
        {
            #region Contracts

            if (hostBuilder == null) throw new ArgumentException($"{nameof(hostBuilder)}=null");
            if (string.IsNullOrEmpty(hookName) == true) throw new ArgumentException($"{nameof(hookName)}=null");
            if (configureMiddleware == null) throw new ArgumentException($"{nameof(configureMiddleware)}=null");

            #endregion

            // HookMiddleware
            hostBuilder.Services.Add(ServiceDescriptor.Transient<HookMiddleware>(serviceProvider => new HookMiddleware(hookName, configureMiddleware)));

            // Return
            return hostBuilder;
        }
    }

    public static partial class HookMiddlewareExtensions
    {
        // Methods 
        public static IApplicationBuilder UseHook(this IApplicationBuilder applicationBuilder, string hookName)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (string.IsNullOrEmpty(hookName) == true) throw new ArgumentException($"{nameof(hookName)}=null");

            #endregion

            // Application
            var application = applicationBuilder as WebApplication;
            if (application == null) throw new InvalidOperationException($"{nameof(application)}=null");

            // UseHook
            return application.UseHook(hookName);
        }

        public static WebApplication UseHook(this WebApplication application, string hookName)
        {
            #region Contracts

            if (application == null) throw new ArgumentException($"{nameof(application)}=null");
            if (string.IsNullOrEmpty(hookName) == true) throw new ArgumentException($"{nameof(hookName)}=null");

            #endregion

            // HookMiddlewareList
            var hookMiddlewareList = application.Services.GetRequiredService<IEnumerable<HookMiddleware>>()?.TakeWhile(o => o.HookName == hookName);
            if (hookMiddlewareList == null) throw new InvalidOperationException($"{nameof(hookMiddlewareList)}=null");
            
            // ConfigureMiddleware
            foreach (var hookMiddleware in hookMiddlewareList)
            {
                hookMiddleware.ConfigureMiddleware(application);
            }

            // Return
            return application;
        }
    }
}
