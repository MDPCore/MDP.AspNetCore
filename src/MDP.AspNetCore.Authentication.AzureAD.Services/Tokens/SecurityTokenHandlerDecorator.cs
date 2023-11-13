using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;

namespace MDP.AspNetCore.Authentication.Jwt
{
    internal class SecurityTokenHandlerDecorator : ISecurityTokenValidator
    {
        // Fields
        private readonly ISecurityTokenValidator _securityTokenValidator = null;


        // Constructors
        public SecurityTokenHandlerDecorator(ISecurityTokenValidator securityTokenValidator) : base()
        {
            #region Contracts

            if (securityTokenValidator == null) throw new ArgumentException($"{nameof(securityTokenValidator)}=null");

            #endregion

            // Default
            _securityTokenValidator = securityTokenValidator;
        }


        // Properties
        public bool CanValidateToken { get => _securityTokenValidator.CanValidateToken; }

        public int MaximumTokenSizeInBytes { get => _securityTokenValidator.MaximumTokenSizeInBytes; set => _securityTokenValidator.MaximumTokenSizeInBytes = value; }


        // Methods
        public bool CanReadToken(string securityToken) { return _securityTokenValidator.CanReadToken(securityToken); }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validateParameters, out SecurityToken validatedToken)
        {
            #region Contracts

            if (string.IsNullOrEmpty(token) == true) throw new ArgumentException($"{nameof(token)}=null");
            if (validateParameters == null) throw new ArgumentException($"{nameof(validateParameters)}=null");

            #endregion

            // ClaimsPrincipal
            var claimsPrincipal = _securityTokenValidator.ValidateToken(token, validateParameters, out validatedToken);
            if (claimsPrincipal == null) return claimsPrincipal;

            // ClaimsIdentity
            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return claimsPrincipal;

            // AuthenticationType
            var authenticationType = string.Empty;
            if (string.IsNullOrEmpty(authenticationType) == true) authenticationType = claimsIdentity.FindFirst(SecurityTokenClaimTypes.AuthenticationType)?.Value;
            if (string.IsNullOrEmpty(authenticationType) == true) return claimsPrincipal;

            // Create
            claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, authenticationType);
            claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Return
            return claimsPrincipal;
        }
    }
}