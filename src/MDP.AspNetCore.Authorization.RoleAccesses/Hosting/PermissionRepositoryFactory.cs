using MDP.Registration;
using MDP.RoleAccesses;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization.RoleAccesses
{
    public class PermissionRepositoryFactory : ServiceFactory<WebApplicationBuilder, PermissionRepositoryFactory.Setting>
    {
        // Constructors
        public PermissionRepositoryFactory() : base("Authorization", "RoleAccesses") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // PermissionRepository
            applicationBuilder.Services.AddTransient<PermissionRepository>((serviceProvider) =>
            {
                // Create
                PermissionRepository permissionRepository = null;
                permissionRepository = new MemoryPermissionRepository(setting.Permissions.Select(o => o.ToPermission()).ToList());
                permissionRepository = new CachePermissionRepository(permissionRepository);

                // Return
                return permissionRepository;
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public List<PermissionSetting> Permissions { get; set; } = null;
        }

        public class PermissionSetting
        {
            // Properties
            public string RoleId { get; set; }

            public string AccessUri { get; set; }


            // Methods
            public Permission ToPermission()
            {
                return new Permission(Guid.NewGuid().ToString(), this.RoleId, this.AccessUri);
            }
        }
    }
}
