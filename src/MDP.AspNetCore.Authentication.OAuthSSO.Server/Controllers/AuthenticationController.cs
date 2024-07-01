using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using MDP.Security.Tokens.Jwt;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Cryptography;
using System.Security.Principal;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class AuthenticationController : Controller
    {
        // Fields
        private readonly AuthenticationControllerSetting _authenticationSetting = null;

        private readonly IDataProtectionProvider _dataProtectionProvider = null;

        private readonly TokenProviderFactory _tokenProviderFactory = null;


        // Constructors
        public AuthenticationController(AuthenticationControllerSetting authenticationSetting, IDataProtectionProvider dataProtectionProvider, TokenProviderFactory tokenProviderFactory)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(authenticationSetting);
            ArgumentNullException.ThrowIfNull(dataProtectionProvider);
            ArgumentNullException.ThrowIfNull(tokenProviderFactory);

            #endregion

            // Default
            _authenticationSetting = authenticationSetting;
            _dataProtectionProvider = dataProtectionProvider;
            _tokenProviderFactory = tokenProviderFactory;
        }


        // Methods
        [HttpGet]
        [AllowAnonymous]
        [Route("/.sso/authorize")]
        public ActionResult Authorize
        (
            string client_id,
            string redirect_uri,
            string response_type,
            string code_challenge,
            string code_challenge_method,
            string state
        )
        {
            #region Contracts

            if (string.IsNullOrEmpty(client_id) == true) return this.BadRequest($"{nameof(client_id)}=null");
            if (string.IsNullOrEmpty(redirect_uri) == true) return this.BadRequest($"{nameof(redirect_uri)}=null");
            if (string.IsNullOrEmpty(response_type) == true) return this.BadRequest($"{nameof(response_type)}=null");
            if (string.IsNullOrEmpty(code_challenge) == true) return this.BadRequest($"{nameof(code_challenge)}=null");
            if (string.IsNullOrEmpty(code_challenge_method) == true) return this.BadRequest($"{nameof(code_challenge_method)}=null");
            if (string.IsNullOrEmpty(state) == true) return this.BadRequest($"{nameof(state)}=null");

            #endregion

            // Require
            if (this.User?.Identity?.IsAuthenticated != true)
            {
                return this.Challenge(new AuthenticationProperties { RedirectUri = this.Request.GetEncodedUrl() });
            }

            // ClientCredential
            var clientCredential = _authenticationSetting.ClientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(client_id)}={client_id}", state);

            // ClientCredential.Require
            {
                // client_id
                if (client_id.Equals(clientCredential.ClientId, StringComparison.OrdinalIgnoreCase) == false) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(client_id)}={client_id}", state);

                // redirect_uri
                if (redirect_uri.StartsWith(clientCredential.RedirectUri, StringComparison.OrdinalIgnoreCase) == false) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(redirect_uri)}={redirect_uri}", state);

                // response_type
                if (response_type.Equals("code", StringComparison.OrdinalIgnoreCase) == false) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(response_type)}={response_type}", state);

                // code_challenge
                if (code_challenge.Equals("", StringComparison.OrdinalIgnoreCase) == true) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(code_challenge)}={code_challenge}", state);

                // code_challenge_method
                if (code_challenge_method.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(code_challenge_method)}={code_challenge_method}", state);

                // state
            }

            // ClaimsIdentity
            var claimsIdentity = this.User?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity)}=null", state);
            if (claimsIdentity.IsAuthenticated == false) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity.IsAuthenticated)}={claimsIdentity.IsAuthenticated}", state);

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(dataProtector)}=null", state);

            // AuthorizationCode
            var authorizationCode = dataProtector.Protect(JsonSerializer.Serialize(new AuthorizationCodeData
            (
                client_id,
                DateTime.Now.Add(TimeSpan.FromMinutes(_authenticationSetting.AuthorizationCodeExpireMinutes)),
                code_challenge,
                code_challenge_method,
                claimsIdentity
            )));
            if (string.IsNullOrEmpty(authorizationCode) == true) return RedirectWithError(redirect_uri, "server_error", $"{nameof(authorizationCode)}=null", state);

            // Return
            return this.RedirectWithCode(redirect_uri, authorizationCode, state);
        }

        private ActionResult RedirectWithCode(string redirectUri, string authorizationCode, string state)
        {
            #region Contracts

            if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentNullException($"{nameof(redirectUri)}=null");
            if (string.IsNullOrEmpty(authorizationCode) == true) throw new ArgumentNullException($"{nameof(authorizationCode)}=null");
            if (string.IsNullOrEmpty(state) == true) throw new ArgumentNullException($"{nameof(state)}=null");

            #endregion

            // Return
            return this.Redirect($"{redirectUri}?code={Uri.EscapeDataString(authorizationCode)}&state={Uri.EscapeDataString(state)}");
        }

        private ActionResult RedirectWithError(string redirectUri, string error, string errorDescription, string state)
        {
            #region Contracts

            if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentNullException($"{nameof(redirectUri)}=null");
            if (string.IsNullOrEmpty(error) == true) throw new ArgumentNullException($"{nameof(error)}=null");
            if (string.IsNullOrEmpty(errorDescription) == true) throw new ArgumentNullException($"{nameof(errorDescription)}=null");
            if (string.IsNullOrEmpty(state) == true) throw new ArgumentNullException($"{nameof(state)}=null");

            #endregion

            // Return
            return this.Redirect($"{redirectUri}?error={Uri.EscapeDataString(error)}&error_description={Uri.EscapeDataString(errorDescription)}&state={Uri.EscapeDataString(state)}");
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/.sso/token")]
        public async Task<ActionResult> GetToken(string grant_type)
        {
            #region Contracts

            if (string.IsNullOrEmpty(grant_type) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(grant_type)}=null" });

            #endregion

            // AuthorizationCode
            if (grant_type.Equals("authorization_code", StringComparison.OrdinalIgnoreCase) == true)
            {
                return await this.GetTokenWithAuthorizationCode();
            }

            // RefreshToken
            if (grant_type.Equals("refresh_token", StringComparison.OrdinalIgnoreCase) == true)
            {
                return await this.GetTokenWithRefreshToken();
            }

            // Other
            return this.BadRequest(new { error = "invalid_grant", error_description = $"{nameof(grant_type)}={grant_type}" });
        }

        private async Task<ActionResult> GetTokenWithAuthorizationCode()
        {
            // Form
            var form = await this.Request.ReadFormAsync();
            if (form == null) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(form)}=null" });

            // Arguments
            var client_id = form["client_id"].ToString(); if (string.IsNullOrEmpty(client_id) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            var redirect_uri = form["redirect_uri"].ToString(); if (string.IsNullOrEmpty(redirect_uri) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(redirect_uri)}=null" });
            var code = form["code"].ToString(); if (string.IsNullOrEmpty(code) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(code)}=null" });
            var code_verifier = form["code_verifier"].ToString(); if (string.IsNullOrEmpty(code_verifier) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(code_verifier)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // AuthorizationCode
            AuthorizationCodeData authorizationCode = null;
            try
            {
                authorizationCode = JsonSerializer.Deserialize<AuthorizationCodeData>(dataProtector.Unprotect(code));
            }
            catch (Exception exception)
            {
                return BadRequest(new { error = "invalid_grant", error_description = exception.Message });
            }
            if (authorizationCode == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode)}=null" });

            // AuthorizationCode.Require
            {
                // GrantType
                if (authorizationCode.GrantType.Equals("authorization_code", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.GrantType)}={authorizationCode.GrantType}" });

                // ClientId
                if (authorizationCode.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ClientId)}={authorizationCode.ClientId}" });

                // ExpireTime
                if (authorizationCode.ExpireTime <= DateTime.Now) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ExpireTime)}={authorizationCode.ExpireTime}" });

                // CodeChallenge
                if (string.IsNullOrEmpty(authorizationCode.CodeChallenge) == true) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}" });

                // CodeChallengeMethod
                if (authorizationCode.CodeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.CodeChallengeMethod)}=null" });

                // ClaimList
                if (authorizationCode.ClaimList == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ClaimList)}=null" });
            }

            // ClientCredential
            var clientCredential = _authenticationSetting.ClientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(clientCredential)}=null" });

            // ClientCredential.Require
            {
                // client_id
                if (client_id.Equals(clientCredential.ClientId, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(client_id)}={client_id}" });

                // redirect_uri
                if (redirect_uri.StartsWith(clientCredential.RedirectUri, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(redirect_uri)}={redirect_uri}" });

                // code
                if (code.Equals("", StringComparison.OrdinalIgnoreCase) == true) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(code)}=null" });

                // code_verifier
                string codeVerifier = null;
                using (var hashAlgorithm = SHA256.Create())
                {
                    // HashBytes
                    var hashBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(code_verifier));
                    if (hashBytes == null) throw new InvalidOperationException($"{nameof(hashBytes)}=null");

                    // HashString
                    var hashString = Convert.ToBase64String(hashBytes)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');

                    // CodeVerifier
                    codeVerifier = hashString;
                }
                if (string.IsNullOrEmpty(codeVerifier) == true) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(codeVerifier)}=null" });
                if (codeVerifier.Equals(authorizationCode.CodeChallenge) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}" });
            }

            // ClaimsIdentity
            var claimsIdentity = authorizationCode.GetClaimsIdentity();
            if (claimsIdentity == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // Return
            return GetToken(authorizationCode.ClientId, claimsIdentity);
        }

        private async Task<ActionResult> GetTokenWithRefreshToken()
        {
            // Form
            var form = await this.Request.ReadFormAsync();
            if (form == null) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(form)}=null" });

            // Arguments
            var client_id = form["client_id"].ToString(); if (string.IsNullOrEmpty(client_id) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            var token = form["refresh_token"].ToString(); if (string.IsNullOrEmpty(token) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(token)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // RefreshToken
            RefreshTokenData refreshToken = null;
            try
            {
                refreshToken = JsonSerializer.Deserialize<RefreshTokenData>(dataProtector.Unprotect(token));
            }
            catch (Exception exception)
            {
                return BadRequest(new { error = "invalid_grant", error_description = exception.Message });
            }
            if (refreshToken == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(refreshToken)}=null" });

            // RefreshToken.Require
            {
                // GrantType
                if (refreshToken.GrantType.Equals("authorization_code", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(refreshToken.GrantType)}={refreshToken.GrantType}" });

                // ClientId
                if (refreshToken.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(refreshToken.ClientId)}={refreshToken.ClientId}" });

                // ExpireTime
                if (refreshToken.ExpireTime <= DateTime.Now) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(refreshToken.ExpireTime)}={refreshToken.ExpireTime}" });

                // ClaimList
                if (refreshToken.ClaimList == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(refreshToken.ClaimList)}=null" });
            }

            // ClientCredential
            var clientCredential = _authenticationSetting.ClientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(clientCredential)}=null" });

            // AuthenticationProvider
            var authenticationProvider = this.HttpContext.RequestServices.GetService(typeof(AuthenticationProvider)) as AuthenticationProvider ?? new AuthenticationProvider();
            if (authenticationProvider == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authenticationProvider)}=null" });

            // ClaimsIdentity
            var claimsIdentity = refreshToken.GetClaimsIdentity();
            if (claimsIdentity != null) claimsIdentity = authenticationProvider.LocalLogin(claimsIdentity);
            if (claimsIdentity == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // Return
            return GetToken(refreshToken.ClientId, claimsIdentity);
        }

        private ActionResult GetToken(string clientId, ClaimsIdentity claimsIdentity)
        {
            #region Contracts

            if (string.IsNullOrEmpty(clientId) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(clientId)}=null" });
            if (claimsIdentity == null) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity)}=null" });

            #endregion

            // TokenProvider
            var tokenProvider = _tokenProviderFactory.CreateProvider(_authenticationSetting.JwtTokenName);
            if (tokenProvider == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(tokenProvider)}=null" });

            // AccessToken
            var accessToken = tokenProvider.CreateToken(
                claimsIdentity,
                TimeSpan.FromMinutes(_authenticationSetting.AccessTokenExpireMinutes)
            );
            if (string.IsNullOrEmpty(accessToken) == true) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(accessToken)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // RefreshToken
            var refreshToken = dataProtector.Protect(JsonSerializer.Serialize(new RefreshTokenData
            (
                clientId,
                DateTime.Now.Add(TimeSpan.FromMinutes(_authenticationSetting.RefreshTokenExpireMinutes)),
                claimsIdentity
            )));
            if (string.IsNullOrEmpty(refreshToken) == true) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(refreshToken)}=null" });

            // Return
            return Ok(new
            {
                token_type = "Bearer",
                access_token = accessToken,
                refresh_token = refreshToken,
                expires_in = _authenticationSetting.AccessTokenExpireMinutes * 60
            });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/.sso/userinfo")]
        public async Task<ActionResult> GetUser()
        {
            // PolicyAuthenticationSelectorList
            var policyAuthenticationSelectorList = this.HttpContext.RequestServices.GetService(typeof(IList<PolicyAuthenticationSelector>)) as IList<PolicyAuthenticationSelector>;
            if (policyAuthenticationSelectorList == null) throw new InvalidOperationException($"{nameof(policyAuthenticationSelectorList)}=null");

            // PolicyAuthenticationSelectorList.Foreach
            var authenticationScheme = string.Empty;
            foreach (var policyAuthenticationSelector in policyAuthenticationSelectorList)
            {
                // Check
                if (policyAuthenticationSelector.Check(this.HttpContext) == false) continue;

                // AuthenticationScheme
                authenticationScheme = policyAuthenticationSelector.AuthenticationScheme;

                // Break
                break;
            }
            if (string.IsNullOrEmpty(authenticationScheme) == true) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(authenticationScheme)}=null" });

            // ClaimsIdentity
            var claimsIdentity = (await this.HttpContext.AuthenticateAsync(authenticationScheme))?.Principal?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // UserInfo
            var userInfo = new Dictionary<string, object>();
            foreach (var claim in new ClaimsBaseData(claimsIdentity).ClaimList)
            {
                if (userInfo.ContainsKey(claim.Key))
                {
                    if (userInfo[claim.Key] is List<string> list)
                    {
                        list.Add(claim.Value);
                    }
                    else
                    {
                        userInfo[claim.Key] = new List<string> { userInfo[claim.Key].ToString(), claim.Value };
                    }
                }
                else
                {
                    userInfo[claim.Key] = claim.Value;
                }
            }

            // Return
            return this.Json(userInfo);
        }


        // Class
        private class RefreshTokenData : ClaimsBaseData
        {
            // Constructors
            public RefreshTokenData() : base()
            {

            }

            public RefreshTokenData
            (
                string clientId,
                DateTime expireTime,
                ClaimsIdentity claimsIdentity
            )
            : base(claimsIdentity)
            {
                // Default
                this.GrantType = "refresh_token";
                this.ClientId = clientId;
                this.ExpireTime = expireTime;
            }


            // Properties
            public string GrantType { get; set; } = null;

            public string ClientId { get; set; } = null;

            public DateTime ExpireTime { get; set; } = DateTime.MinValue;
        }

        private class AuthorizationCodeData : ClaimsBaseData
        {
            // Constructors
            public AuthorizationCodeData() : base()
            {

            }

            public AuthorizationCodeData
            (
                string clientId,
                DateTime expireTime,
                string codeChallenge,
                string codeChallengeMethod,
                ClaimsIdentity claimsIdentity
            )
            : base(claimsIdentity)
            {
                #region Contracts

                if (string.IsNullOrEmpty(clientId) == true) throw new ArgumentNullException($"{nameof(clientId)}=null");
                if (string.IsNullOrEmpty(codeChallenge) == true) throw new ArgumentNullException($"{nameof(codeChallenge)}=null");
                if (string.IsNullOrEmpty(codeChallengeMethod) == true) throw new ArgumentNullException($"{nameof(codeChallengeMethod)}=null");

                #endregion

                // Default
                this.GrantType = "authorization_code";
                this.ClientId = clientId;
                this.ExpireTime = expireTime;
                this.CodeChallenge = codeChallenge;
                this.CodeChallengeMethod = codeChallengeMethod;
            }


            // Properties
            public string GrantType { get; set; } = null;

            public string ClientId { get; set; } = null;

            public DateTime ExpireTime { get; set; } = DateTime.MinValue;

            public string CodeChallenge { get; set; } = null;

            public string CodeChallengeMethod { get; set; } = null;
        }

        private class ClaimsBaseData
        {
            // Constructors
            public ClaimsBaseData()
            {
                // ClaimList
                this.ClaimList = new List<KeyValuePair<string, string>>();
            }

            public ClaimsBaseData(ClaimsIdentity claimsIdentity)
            {
                #region Contracts

                if (claimsIdentity == null) throw new ArgumentNullException($"{nameof(claimsIdentity)}=null");

                #endregion

                // ClaimList
                this.ClaimList = claimsIdentity.Claims.Select(claim => new KeyValuePair<string, string>(claim.Type, claim.Value)).ToList();
                this.RemoveClaim(AuthenticationClaimTypes.AuthenticationType);
                this.AddClaim(AuthenticationClaimTypes.AuthenticationType, claimsIdentity.AuthenticationType);
            }


            // Properties
            public List<KeyValuePair<string, string>> ClaimList { get; set; } = null;


            // Methods
            public void AddClaim(string key, object value)
            {
                // Require
                if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException($"{nameof(key)}=null");

                // Add
                this.ClaimList.Add(new KeyValuePair<string, string>(key, value as string));
            }

            public void RemoveClaim(string key)
            {
                // Require
                if (string.IsNullOrEmpty(key) == true) throw new ArgumentNullException($"{nameof(key)}=null");

                // Remove
                this.ClaimList.RemoveAll(o => o.Key == key);
            }

            public ClaimsIdentity GetClaimsIdentity()
            {
                // ClaimList
                var claimList = this.ClaimList.Where(o => o.Key != AuthenticationClaimTypes.AuthenticationType).Select(o => new Claim(o.Key, o.Value)).ToList();
                if (claimList == null) throw new InvalidOperationException($"{nameof(claimList)}=null");

                // AuthenticationType
                var authenticationType = this.ClaimList.FirstOrDefault(o => o.Key == AuthenticationClaimTypes.AuthenticationType).Value as string;
                if (string.IsNullOrEmpty(authenticationType) == true) throw new InvalidOperationException($"{nameof(authenticationType)}=null");

                // Return
                return new ClaimsIdentity(claimList, authenticationType);
            }
        }
    }
}
