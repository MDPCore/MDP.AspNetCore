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

            // HmacToken
            var hmacToken = securityTokenFactory.CreateSecurityToken("HmacToken", claimsIdentity);
            if (string.IsNullOrEmpty(hmacToken) == true) throw new InvalidOperationException($"{nameof(hmacToken)}=null");
            Console.WriteLine(hmacToken);
            Console.WriteLine();

            // RsaToken
            var rsaToken = securityTokenFactory.CreateSecurityToken("RsaToken", claimsIdentity);
            if (string.IsNullOrEmpty(rsaToken) == true) throw new InvalidOperationException($"{nameof(rsaToken)}=null");
            Console.WriteLine(rsaToken);
            Console.WriteLine();
        }

        public static void Main(string[] args)
        {
            // Host
            MDP.NetCore.Host.Run<Program>(args);
        }
    }
}
