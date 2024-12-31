using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Apple
{
    public class AppleOptions : OAuthOptions
    {
        // Constructors
        public AppleOptions()
        {
            // Options
            this.CallbackPath = new PathString("/signin-apple");
            this.AuthorizationEndpoint = AppleDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = AppleDefaults.TokenEndpoint;

            // Scope
            this.Scope.Add("openid");
            this.Scope.Add("name");
            this.Scope.Add("email");
        }


        // Properties
        public string ServiceId { get { return this.ClientId; } set { this.ClientId = value; } } 

        public string TeamId { get; set; } = string.Empty;

        public string KeyId { get; set; } = string.Empty;

        public string SignKey { get; set; } = string.Empty;


        // Methods
        public string CreateClientSecret()
        {
            // ClaimList
            var claimList = new List<Claim>();
            {
                claimList.Add(new Claim("sub", this.ServiceId));
            }

            // SigningCredentials
            var ecdsaKey = ECDsa.Create();
            {
                ecdsaKey.ImportFromPem(this.SignKey);
            }
            var securityKey = new ECDsaSecurityKey(ecdsaKey);
            {
                securityKey.KeyId = this.KeyId;
            }
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

            // TokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Claim
                Subject = new ClaimsIdentity(claimList),

                // Issuer
                Issuer = this.TeamId,
                Audience = "https://appleid.apple.com",

                // Lifetime
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(30),

                // Signing
                SigningCredentials = signingCredentials
            };

            // ClientSecret
            var clientSecret = (new JwtSecurityTokenHandler()).CreateEncodedJwt(tokenDescriptor);
            if (string.IsNullOrEmpty(clientSecret) == true) throw new InvalidOperationException($"{nameof(clientSecret)}=null");

            // Return
            return clientSecret;
        }
    }
}
