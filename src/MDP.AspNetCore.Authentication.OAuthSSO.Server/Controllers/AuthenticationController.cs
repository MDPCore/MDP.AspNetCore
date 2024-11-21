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
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    [ApiExplorerSettings(IgnoreApi = true)]
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

            if (string.IsNullOrEmpty(client_id) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            if (string.IsNullOrEmpty(redirect_uri) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(redirect_uri)}=null" });
            if (string.IsNullOrEmpty(response_type) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(response_type)}=null" });
            if (string.IsNullOrEmpty(code_challenge) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(code_challenge)}=null" });
            if (string.IsNullOrEmpty(code_challenge_method) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(code_challenge_method)}=null" });
            if (string.IsNullOrEmpty(state) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(state)}=null" });

            #endregion

            // Require
            if (this.User?.Identity?.IsAuthenticated != true)
            {
                return this.Challenge(new AuthenticationProperties { RedirectUri = this.Request.GetEncodedUrl() });
            }
            redirect_uri = WebUtility.UrlDecode(redirect_uri);

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
                if (state.Equals("", StringComparison.OrdinalIgnoreCase) == true) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(state)}={state}", state);
            }

            // ClaimsIdentity
            var claimsIdentity = this.User?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity)}=null", state);
            if (claimsIdentity.IsAuthenticated == false) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity.IsAuthenticated)}={claimsIdentity.IsAuthenticated}", state);

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(dataProtector)}=null", state);

            // AuthorizationCode
            string authorizationCode = null;
            try
            {
                // AuthorizationCodeData
                var authorizationCodeData = new AuthorizationCodeData();
                {
                    authorizationCodeData.GrantType = "authorization_code";
                    authorizationCodeData.ClientId = client_id;
                    authorizationCodeData.SetClaimsIdentity(claimsIdentity);
                    authorizationCodeData.ExpireTime = DateTime.Now.Add(TimeSpan.FromMinutes(_authenticationSetting.AuthorizationCodeExpireMinutes));
                    authorizationCodeData.CodeChallenge = code_challenge;
                    authorizationCodeData.CodeChallengeMethod = code_challenge_method;
                }

                // Serialize
                authorizationCode = dataProtector.Protect(JsonSerializer.Serialize(authorizationCodeData, new JsonSerializerOptions
                {
                    Converters = { new ClaimConverter() }
                }));
            }
            catch (Exception exception)
            {
                return RedirectWithError(redirect_uri, "server_error", $"errorMessage={exception.Message}", state);
            }
            if (string.IsNullOrEmpty(authorizationCode) == true) return RedirectWithError(redirect_uri, "server_error", $"{nameof(authorizationCode)}=null", state);

            // Return
            return this.Redirect($"{redirect_uri}?code={Uri.EscapeDataString(authorizationCode)}&state={Uri.EscapeDataString(state)}");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/.sso/logout")]
        public async Task<ActionResult> Logout
        (
            string client_id,
            string redirect_uri,
            string state
        )
        {
            #region Contracts

            if (string.IsNullOrEmpty(client_id) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            if (string.IsNullOrEmpty(redirect_uri) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(redirect_uri)}=null" });
            if (string.IsNullOrEmpty(state) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(state)}=null" });

            #endregion

            // Require
            if (this.User?.Identity?.IsAuthenticated != false)
            {
                return await this.LogoutAsync(this.Request.GetEncodedUrl());
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

                // state
                if (state.Equals("", StringComparison.OrdinalIgnoreCase) == true) return RedirectWithError(redirect_uri, "invalid_request", $"{nameof(state)}={state}", state);
            }

            // Return
            return this.Redirect($"{redirect_uri}?state={Uri.EscapeDataString(state)}");
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

            if (string.IsNullOrEmpty(grant_type) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(grant_type)}=null" });

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
            return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(grant_type)}={grant_type}" });
        }

        private async Task<ActionResult> GetTokenWithAuthorizationCode()
        {
            // Form
            var form = await this.Request.ReadFormAsync();
            if (form == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(form)}=null" });

            // Arguments
            var client_id = form["client_id"].ToString(); if (string.IsNullOrEmpty(client_id) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            var redirect_uri = form["redirect_uri"].ToString(); if (string.IsNullOrEmpty(redirect_uri) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(redirect_uri)}=null" });
            var code = form["code"].ToString(); if (string.IsNullOrEmpty(code) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(code)}=null" });
            var code_verifier = form["code_verifier"].ToString(); if (string.IsNullOrEmpty(code_verifier) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(code_verifier)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // AuthorizationCode
            AuthorizationCodeData authorizationCode = null;
            try
            {
                // Deserialize
                authorizationCode = JsonSerializer.Deserialize<AuthorizationCodeData>(dataProtector.Unprotect(code), new JsonSerializerOptions
                {
                    Converters = { new ClaimConverter() }
                });
            }
            catch (Exception exception)
            {
                return this.BadRequest(new { error = "invalid_request", error_description = $"errorMessage={exception.Message}" });
            }
            if (authorizationCode == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode)}=null" });

            // AuthorizationCode.Require
            {
                // GrantType
                if (authorizationCode.GrantType.Equals("authorization_code", StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.GrantType)}={authorizationCode.GrantType}" });

                // ClientId
                if (authorizationCode.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.ClientId)}={authorizationCode.ClientId}" });

                // ClaimList
                if (authorizationCode.ClaimList == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.ClaimList)}=null" });

                // ExpireTime
                if (authorizationCode.ExpireTime <= DateTime.Now) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.ExpireTime)}={authorizationCode.ExpireTime}" });

                // CodeChallenge
                if (string.IsNullOrEmpty(authorizationCode.CodeChallenge) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}" });

                // CodeChallengeMethod
                if (authorizationCode.CodeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.CodeChallengeMethod)}=null" });
            }

            // ClientCredential
            var clientCredential = _authenticationSetting.ClientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(clientCredential)}=null" });

            // ClientCredential.Require
            {
                // client_id
                if (client_id.Equals(clientCredential.ClientId, StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}={client_id}" });

                // redirect_uri
                if (redirect_uri.StartsWith(clientCredential.RedirectUri, StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(redirect_uri)}={redirect_uri}" });

                // code
                if (code.Equals("", StringComparison.OrdinalIgnoreCase) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(code)}=null" });

                // code_verifier
                string codeVerifier = null;
                using (var hashAlgorithm = SHA256.Create())
                {
                    // HashBytes
                    var hashBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(code_verifier));
                    if (hashBytes == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(hashBytes)}=null" });

                    // HashString
                    var hashString = Convert.ToBase64String(hashBytes)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');

                    // CodeVerifier
                    codeVerifier = hashString;
                }
                if (string.IsNullOrEmpty(codeVerifier) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(codeVerifier)}=null" });
                if (codeVerifier.Equals(authorizationCode.CodeChallenge) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}" });
            }

            // AuthenticationProvider
            var authenticationProvider = this.HttpContext.RequestServices.GetService(typeof(AuthenticationProvider)) as AuthenticationProvider ?? new AuthenticationProvider();
            if (authenticationProvider == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(authenticationProvider)}=null" });

            // ClaimsIdentity
            var claimsIdentity = authorizationCode.GetClaimsIdentity();
            if (claimsIdentity != null) claimsIdentity = authenticationProvider.LocalLogin(claimsIdentity);
            if (claimsIdentity == null) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // Return
            return GetToken(authorizationCode.ClientId, claimsIdentity);
        }

        private async Task<ActionResult> GetTokenWithRefreshToken()
        {
            // Form
            var form = await this.Request.ReadFormAsync();
            if (form == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(form)}=null" });

            // Arguments
            var client_id = form["client_id"].ToString(); if (string.IsNullOrEmpty(client_id) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(client_id)}=null" });
            var token = form["refresh_token"].ToString(); if (string.IsNullOrEmpty(token) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(token)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // RefreshToken
            RefreshTokenData refreshToken = null;
            try
            {
                // Deserialize
                refreshToken = JsonSerializer.Deserialize<RefreshTokenData>(dataProtector.Unprotect(token), new JsonSerializerOptions
                {
                    Converters = { new ClaimConverter() }
                });
            }
            catch (Exception exception)
            {
                return this.BadRequest(new { error = "invalid_request", error_description = $"errorMessage={exception.Message}" });
            }
            if (refreshToken == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(refreshToken)}=null" });

            // RefreshToken.Require
            {
                // GrantType
                if (refreshToken.GrantType.Equals("refresh_token", StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(refreshToken.GrantType)}={refreshToken.GrantType}" });

                // ClientId
                if (refreshToken.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(refreshToken.ClientId)}={refreshToken.ClientId}" });

                // ClaimList
                if (refreshToken.ClaimList == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(refreshToken.ClaimList)}=null" });

                // ExpireTime
                if (refreshToken.ExpireTime <= DateTime.Now) return Unauthorized(new { error = "invalid_request", error_description = $"{nameof(refreshToken.ExpireTime)}={refreshToken.ExpireTime}" });
            }

            // ClientCredential
            var clientCredential = _authenticationSetting.ClientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(clientCredential)}=null" });

            // AuthenticationProvider
            var authenticationProvider = this.HttpContext.RequestServices.GetService(typeof(AuthenticationProvider)) as AuthenticationProvider ?? new AuthenticationProvider();
            if (authenticationProvider == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(authenticationProvider)}=null" });

            // ClaimsIdentity
            var claimsIdentity = refreshToken.GetClaimsIdentity();
            if (claimsIdentity != null) claimsIdentity = authenticationProvider.LocalLogin(claimsIdentity);
            if (claimsIdentity == null) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // Return
            return GetToken(refreshToken.ClientId, claimsIdentity);
        }

        private ActionResult GetToken(string clientId, ClaimsIdentity claimsIdentity)
        {
            #region Contracts

            if (string.IsNullOrEmpty(clientId) == true) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(clientId)}=null" });
            if (claimsIdentity == null) return this.BadRequest(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity)}=null" });

            #endregion

            // NowTime
            var nowTime = DateTime.Now;

            // TokenProvider
            var tokenProvider = _tokenProviderFactory.CreateProvider(_authenticationSetting.JwtTokenName);
            if (tokenProvider == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(tokenProvider)}=null" });

            // AccessToken
            var accessToken = tokenProvider.CreateToken(
                claimsIdentity,
                TimeSpan.FromMinutes(_authenticationSetting.AccessTokenExpireMinutes)
            );
            if (string.IsNullOrEmpty(accessToken) == true) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(accessToken)}=null" });

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // RefreshToken
            string refreshToken = null;
            try
            {
                // RefreshTokenData
                var refreshTokenData = new RefreshTokenData();
                {
                    refreshTokenData.GrantType = "refresh_token";
                    refreshTokenData.ClientId = clientId;
                    refreshTokenData.SetClaimsIdentity(claimsIdentity);
                    refreshTokenData.ExpireTime = nowTime.Add(TimeSpan.FromMinutes(_authenticationSetting.RefreshTokenExpireMinutes));
                }

                // Serialize
                refreshToken = dataProtector.Protect(JsonSerializer.Serialize(refreshTokenData, new JsonSerializerOptions
                {
                    Converters = { new ClaimConverter() }
                }));
            }
            catch (Exception exception)
            {
                return this.StatusCode(500, new { error = "server_error", error_description = $"errorMessage={exception.Message}" });
            }
            if (string.IsNullOrEmpty(refreshToken) == true) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(refreshToken)}=null" });

            // Return
            return Ok(new
            {
                token_type = "Bearer",
                expires_in = _authenticationSetting.AccessTokenExpireMinutes * 60,

                access_token = accessToken,
                access_token_expiration = nowTime.Add(TimeSpan.FromMinutes(_authenticationSetting.AccessTokenExpireMinutes)),

                refresh_token = refreshToken,
                refresh_token_expiration = nowTime.Add(TimeSpan.FromMinutes(_authenticationSetting.RefreshTokenExpireMinutes))
            });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/.sso/userinfo")]
        public async Task<ActionResult> GetUser()
        {
            // PolicyAuthenticationSelectorList
            var policyAuthenticationSelectorList = this.HttpContext.RequestServices.GetService(typeof(IList<PolicyAuthenticationSelector>)) as IList<PolicyAuthenticationSelector>;
            if (policyAuthenticationSelectorList == null) return this.StatusCode(500, new { error = "server_error", error_description = $"{nameof(policyAuthenticationSelectorList)}=null" });

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
            if (claimsIdentity == null) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity)}=null" });
            if (claimsIdentity.IsAuthenticated == false) return this.Unauthorized(new { error = "invalid_request", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

            // ClaimList
            var claimList = claimsIdentity.Claims.ToList();
            claimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);
            claimList.Add(new Claim(AuthenticationClaimTypes.AuthenticationType, claimsIdentity.AuthenticationType));

            // UserInfo
            var userInfo = new Dictionary<string, object>();
            foreach (var claim in claimList)
            {
                if (userInfo.ContainsKey(claim.Type))
                {
                    if (userInfo[claim.Type] is List<string> list)
                    {
                        list.Add(claim.Value);
                    }
                    else
                    {
                        userInfo[claim.Type] = new List<string> { userInfo[claim.Type].ToString(), claim.Value };
                    }
                }
                else
                {
                    userInfo[claim.Type] = claim.Value;
                }
            }

            // Return
            return this.Json(userInfo);
        }


        // Class
        private class AuthorizationCodeData
        {
            // Properties
            public string GrantType { get; set; } = null;

            public string ClientId { get; set; } = null;

            public List<Claim> ClaimList { get; set; } = null;

            public DateTime ExpireTime { get; set; } = DateTime.MinValue;

            public string CodeChallenge { get; set; } = null;

            public string CodeChallengeMethod { get; set; } = null;


            // Methods
            public void SetClaimsIdentity(ClaimsIdentity claimsIdentity)
            {
                #region Contracts

                ArgumentNullException.ThrowIfNull(claimsIdentity);

                #endregion

                // ClaimList
                this.ClaimList = claimsIdentity.Claims.ToList();
                this.ClaimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);
                this.ClaimList.Add(new Claim(AuthenticationClaimTypes.AuthenticationType, claimsIdentity.AuthenticationType));
            }

            public ClaimsIdentity GetClaimsIdentity()
            {
                // ClaimList
                var claimList = this.ClaimList?.ToList();
                if (claimList == null) throw new InvalidOperationException($"{nameof(claimList)}=null");
                if (claimList != null) claimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);

                // AuthenticationType
                var authenticationType = this.ClaimList?.FirstOrDefault(o => o.Type == AuthenticationClaimTypes.AuthenticationType)?.Value;
                if (string.IsNullOrEmpty(authenticationType) == true) throw new InvalidOperationException($"{nameof(authenticationType)}=null");

                // Return
                return new ClaimsIdentity(claimList, authenticationType);
            }
        }

        private class RefreshTokenData
        {
            // Properties
            public string GrantType { get; set; } = null;

            public string ClientId { get; set; } = null;

            public List<Claim> ClaimList { get; set; } = null;

            public DateTime ExpireTime { get; set; } = DateTime.MinValue;


            // Methods
            public void SetClaimsIdentity(ClaimsIdentity claimsIdentity)
            {
                #region Contracts

                ArgumentNullException.ThrowIfNull(claimsIdentity);

                #endregion

                // ClaimList
                this.ClaimList = claimsIdentity.Claims.ToList();
                this.ClaimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);
                this.ClaimList.Add(new Claim(AuthenticationClaimTypes.AuthenticationType, claimsIdentity.AuthenticationType));
            }

            public ClaimsIdentity GetClaimsIdentity()
            {
                // ClaimList
                var claimList = this.ClaimList?.ToList();
                if (claimList == null) throw new InvalidOperationException($"{nameof(claimList)}=null");
                if (claimList != null) claimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);

                // AuthenticationType
                var authenticationType = this.ClaimList?.FirstOrDefault(o => o.Type == AuthenticationClaimTypes.AuthenticationType)?.Value;
                if (string.IsNullOrEmpty(authenticationType) == true) throw new InvalidOperationException($"{nameof(authenticationType)}=null");

                // Return
                return new ClaimsIdentity(claimList, authenticationType);
            }
        }

        private class ClaimConverter: JsonConverter<Claim>
        {
            // Methods
            public override Claim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                #region Contracts

                ArgumentNullException.ThrowIfNull(typeToConvert);
                ArgumentNullException.ThrowIfNull(options);

                #endregion

                // Require
                if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

                // Red
                var type = string.Empty;
                var value = string.Empty;
                while (reader.Read())
                {
                    // End
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        // Require
                        if (string.IsNullOrEmpty(type) == true) throw new JsonException();
                        if (string.IsNullOrEmpty(value) == true) throw new JsonException();

                        // Create
                        var claim = new Claim(type, value);
                        type = string.Empty;
                        value = string.Empty;

                        // Return
                        return claim;
                    }

                    // Property
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        // PropertyName
                        string propertyName = reader.GetString();
                        reader.Read();

                        // PropertyValue
                        switch (propertyName)
                        {
                            case "Type":
                                type = reader.GetString();
                                break;
                            case "Value":
                                value = reader.GetString();
                                break;
                        }
                    }
                }

                // Throw
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Claim claim, JsonSerializerOptions options)
            {
                #region Contracts

                ArgumentNullException.ThrowIfNull(writer);
                ArgumentNullException.ThrowIfNull(claim);
                ArgumentNullException.ThrowIfNull(options);

                #endregion

                writer.WriteStartObject();
                writer.WriteString("Type", claim.Type);
                writer.WriteString("Value", claim.Value);
                writer.WriteEndObject();
            }
        }
    }
}