using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MDP.RoleAccesses.Lab
{
    public class Program
    {
        // Methods
        public static void Run(RoleAccessesContext accessesContext)
        {
            #region Contracts

            if (accessesContext == null) throw new ArgumentException($"{nameof(accessesContext)}=null");

            #endregion

            // True
            Console.WriteLine("HasAccess=True");
            HasAccess(accessesContext, "Admin", "menu://MDP.RBAC.Service/Users/Add");
            HasAccess(accessesContext, "User", "menu://MDP.RBAC.Service/Users/List");
            HasAccess(accessesContext, "User", "menu://MDP.RBAC.Service/Users/12345/Profile");
            Console.WriteLine();

            // False
            Console.WriteLine("HasAccess=False");
            HasAccess(accessesContext, "Admin", "menu://MDP.RBAC.Service/Roles/List");
            HasAccess(accessesContext, "User", "menu://MDP.RBAC.Service/Users/Add");
            Console.WriteLine();
        }

        private static void HasAccess(RoleAccessesContext accessesContext, string roleId, string resourceUri)
        {
            #region Contracts

            if (accessesContext == null) throw new ArgumentException($"{nameof(accessesContext)}=null");
            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(resourceUri) == true) throw new ArgumentException($"{nameof(resourceUri)}=null");

            #endregion

            // Display
            Console.WriteLine($"HasAccess={accessesContext.HasAccess(roleId, resourceUri)}, RoleId={roleId}, Resource={resourceUri}");
        }

        public static void Main(string[] args)
        {
            // Host
            MDP.NetCore.Host.Run<Program>(args);
        }
    }
}
