using MDP.Registration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    public class MemoryPermissionRepositoryFactory : ServiceFactory<IServiceCollection, MemoryPermissionRepositoryFactory.Setting>
    {
        // Constructors
        public MemoryPermissionRepositoryFactory() : base("MDP.RoleAccesses", "MemoryPermissionRepository") { }


        // Methods
        public override void ConfigureService(IServiceCollection serviceCollection, Setting setting)
        {
            #region Contracts

            if (serviceCollection == null) throw new ArgumentException($"{nameof(serviceCollection)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // MemoryPermissionRepository
            serviceCollection.AddSingleton(new ServiceRegistration()
            {
                ServiceType = typeof(PermissionRepository),
                InstanceType = typeof(MemoryPermissionRepository),
                InstanceName = nameof(MemoryPermissionRepository),
                Parameters = new Dictionary<string, object>
                {
                    { "permissionList" , setting.PermissionList.Select(o=> o.ToPermission()).ToList()}
                },
                Singleton = false,
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public List<PermissionSetting> PermissionList { get; set; }
        }

        public class PermissionSetting
        {
            // Properties
            public string PermissionId { get; set; }

            public string RoleId { get; set; }

            public string AccessUri { get; set; }


            // Methods
            public Permission ToPermission()
            {
                return new Permission(this.PermissionId, this.RoleId, this.AccessUri);
            }
        }
    }
}
