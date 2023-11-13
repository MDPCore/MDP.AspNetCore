using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MDP.Security.Tokens.Jwt
{
    [MDP.Registration.Service<SecurityTokenFactory>(singleton: true)]
    public class SecurityTokenFactory
    {
        // Fields
        private readonly JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        private readonly List<SecurityTokenCredential> _securityTokenCredentialList = null;


        // Constructors
        public SecurityTokenFactory(List<SecurityTokenCredential> credentials)
        {
            #region Contracts

            if (credentials==null) throw new ArgumentException($"{nameof(credentials)}=null");

            #endregion

            // Default
            _securityTokenCredentialList = credentials;
        }


        // Methods
        public string CreateSecurityToken(string name, ClaimsIdentity identity, int? expireMinutes = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentException($"{nameof(name)}=null");
            if (identity == null) throw new ArgumentException($"{nameof(identity)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(identity.AuthenticationType) == true) throw new InvalidOperationException($"{nameof(identity.AuthenticationType)}=null");

            // ClaimList
            var claimList = new List<Claim>(identity.Claims);
            {
                // AuthenticationType
                claimList.RemoveAll(claim => claim.Type == SecurityTokenClaimTypes.AuthenticationType);
                claimList.Add(new Claim(SecurityTokenClaimTypes.AuthenticationType, identity.AuthenticationType));
            }

            // CreateSecurityToken
            return this.CreateSecurityToken(name, claimList, expireMinutes);
        }

        public string CreateSecurityToken(string name, IEnumerable<Claim> claims, int? expireMinutes = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentException($"{nameof(name)}=null");
            if (claims == null) throw new ArgumentException($"{nameof(claims)}=null");

            #endregion

            // SecurityTokenCredential
            var securityTokenCredential = _securityTokenCredentialList.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
            if (securityTokenCredential == null) throw new InvalidOperationException($"{nameof(name)}={name} not found");
           
            // SigningCredentials
            var signingCredentials = this.CreareSigningCredentials(securityTokenCredential.Algorithm, securityTokenCredential.SignKey);
            if (signingCredentials == null) throw new InvalidOperationException($"{nameof(securityTokenCredential.Algorithm)}={securityTokenCredential.Algorithm} not found");

            // ClaimList
            var claimList = new List<Claim>(claims);
            {
                // Issuer
                claimList.RemoveAll(claim => claim.Type == JwtRegisteredClaimNames.Iss);
                claimList.Add(new Claim(JwtRegisteredClaimNames.Iss, securityTokenCredential.Issuer));
            }

            // ExpireMinutes
            if (expireMinutes.HasValue == false)
            {
                expireMinutes = securityTokenCredential.ExpireMinutes;
            }

            // SecurityTokenDescriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                // Claim
                Subject = new ClaimsIdentity(claimList),

                // Lifetime
                IssuedAt = DateTime.Now, // 建立時間
                NotBefore = DateTime.Now, // 在此之前不可用時間
                Expires = DateTime.Now.AddMinutes(expireMinutes.Value), // 逾期時間

                // Signing
                SigningCredentials = signingCredentials
            };

            // SecurityToken
            var securityToken = _securityTokenHandler.CreateEncodedJwt(securityTokenDescriptor);
            if (string.IsNullOrEmpty(securityToken) == true) throw new InvalidOperationException($"{nameof(securityToken)}=null");

            // Return
            return securityToken;
        }

        private SigningCredentials CreareSigningCredentials(string algorithm, string signKey)
        {
            #region Contracts

            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentException($"{nameof(signKey)}=null");

            #endregion

            // SecurityKey
            var securityKey = this.CreareSecurityKey(algorithm, signKey);
            if (securityKey == null) return null;

            // Return
            return new SigningCredentials(securityKey, algorithm);
        }

        private SecurityKey CreareSecurityKey(string algorithm, string signKey)
        {
            #region Contracts

            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentException($"{nameof(signKey)}=null");

            #endregion

            // HMAC+SHA
            if (algorithm.StartsWith("HS", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyBytes
                var signKeyBytes = Convert.FromBase64String(signKey);
                if (signKeyBytes == null) throw new InvalidOperationException($"{nameof(signKeyBytes)}=null");

                // SecurityKey
                var securityKey = new SymmetricSecurityKey(signKeyBytes);

                // Return
                return securityKey;
            }

            // RSA+SHA
            if (algorithm.StartsWith("RS", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyString
                var signKeyString = signKey;
                if (string.IsNullOrEmpty(signKeyString) == true) throw new InvalidOperationException($"{nameof(signKeyString)}=null");

                // RsaKey
                var rsaKey = RSA.Create();
                rsaKey.ImportFromPem(signKeyString);

                // SecurityKey
                var securityKey = new RsaSecurityKey(rsaKey);

                // Return
                return securityKey;
            }

            // ECDSA+SHA
            if (algorithm.StartsWith("ES", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyString
                var signKeyString = signKey;
                if (string.IsNullOrEmpty(signKeyString) == true) throw new InvalidOperationException($"{nameof(signKeyString)}=null");

                // EcdsaKey
                var ecdsaKey = ECDsa.Create();
                ecdsaKey.ImportFromPem(signKeyString);

                // SecurityKey
                var securityKey = new ECDsaSecurityKey(ecdsaKey);

                // Return
                return securityKey;
            }

            // Other
            return null;
        }
    }
}
