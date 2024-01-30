﻿using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication
{
    public class DefaultAuthenticationFactory : ServiceFactory<WebApplicationBuilder, DefaultAuthenticationSetting>
    {
        // Constructors
        public DefaultAuthenticationFactory() : base("Authentication") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, DefaultAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddDefaultAuthentication
            webApplicationBuilder.Services.AddDefaultAuthentication(authenticationSetting);
        }
    }
}