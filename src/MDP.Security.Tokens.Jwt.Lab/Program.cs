using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MDP.Security.Tokens.Jwt.Lab
{
    public class Program
    {
        // Methods
        public static void Run(SecurityTokenFactory securityTokenFactory)
        {
            #region Contracts

            if (securityTokenFactory == null) throw new ArgumentException($"{nameof(securityTokenFactory)}=null");

            #endregion

            // Variables
            var username = "Clark";

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(System.Security.Claims.ClaimTypes.Name, username),
                new Claim(System.Security.Claims.ClaimTypes.Email, username + "@hotmail.com"),
            }, "Password");

            // SecurityToken
            var securityToken = securityTokenFactory.CreateSecurityToken("HMACToken", claimsIdentity);
            if (string.IsNullOrEmpty(securityToken) == true) throw new InvalidOperationException($"{nameof(securityToken)}=null");

            // Execute
            Console.WriteLine(securityToken);
        }

        public static void Main(string[] args)
        {
            // Host
            MDP.NetCore.Host.Run<Program>(args);
        }
    }
}
