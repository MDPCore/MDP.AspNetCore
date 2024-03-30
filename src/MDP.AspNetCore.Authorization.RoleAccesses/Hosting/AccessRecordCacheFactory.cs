using MDP.Registration;
using MDP.RoleAccesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore.Authorization.RoleAccesses
{
    public class CacheAccessRecordRepositoryFactory : ServiceFactory<WebApplicationBuilder, CacheAccessRecordRepositoryFactory.Setting>
    {
        // Constructors
        public CacheAccessRecordRepositoryFactory() : base("Authorization", "RoleAccesses") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // AccessRecordRepository
            applicationBuilder.Services.AddTransient<AccessRecordRepository>((serviceProvider) =>
            {
                // Create
                AccessRecordRepository accessRecordRepository = null;
                accessRecordRepository = new CacheAccessRecordRepository();

                // Return
                return accessRecordRepository;
            });
        }


        // Class
        public class Setting
        {
            
        }
    }
}
