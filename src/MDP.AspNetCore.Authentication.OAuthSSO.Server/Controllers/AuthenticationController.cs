using MDP.Security.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public partial class AuthenticationController : Controller
    {
        // Fields
        private readonly AuthenticationControllerSetting _authenticationSetting = null;

        private readonly IDataProtectionProvider _dataProtectionProvider = null;


        // Constructors
        public AuthenticationController(AuthenticationControllerSetting authenticationSetting, IDataProtectionProvider dataProtectionProvider)
        {
            #region Contracts

            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");
            if (dataProtectionProvider == null) throw new ArgumentException($"{nameof(dataProtectionProvider)}=null");

            #endregion

            // Default
            _authenticationSetting = authenticationSetting;
            _dataProtectionProvider = dataProtectionProvider;
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

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(dataProtector)}=null", state);

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

            // AuthorizationCode
            string authorizationCode = null;
            {
                // ClaimsIdentity
                var claimsIdentity = this.User?.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity)}=null", state);
                if (claimsIdentity.IsAuthenticated == false) return RedirectWithError(redirect_uri, "server_error", $"{nameof(claimsIdentity.IsAuthenticated)}={claimsIdentity.IsAuthenticated}", state);

                // AuthorizationCode
                authorizationCode = dataProtector.Protect(JsonSerializer.Serialize(new AuthorizationCodeData
                (
                    client_id,
                    code_challenge,
                    code_challenge_method,
                    claimsIdentity,
                    DateTime.Now.AddMinutes(_authenticationSetting.ExpirationMinutes)
                )));
            }
            if (string.IsNullOrEmpty(authorizationCode) == true) return RedirectWithError(redirect_uri, "server_error", $"{nameof(authorizationCode)}=null", state);

            // Return
            return this.RedirectWithCode(redirect_uri, authorizationCode, state);
        }

        private ActionResult RedirectWithCode(string redirectUri, string authorizationCode, string state)
        {
            #region Contracts

            if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentException($"{nameof(redirectUri)}=null");
            if (string.IsNullOrEmpty(authorizationCode) == true) throw new ArgumentException($"{nameof(authorizationCode)}=null");
            if (string.IsNullOrEmpty(state) == true) throw new ArgumentException($"{nameof(state)}=null");

            #endregion

            // Return
            return this.Redirect($"{redirectUri}?code={Uri.EscapeDataString(authorizationCode)}&state={Uri.EscapeDataString(state)}");
        }

        private ActionResult RedirectWithError(string redirectUri, string error, string errorDescription, string state)
        {
            #region Contracts

            if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentException($"{nameof(redirectUri)}=null");
            if (string.IsNullOrEmpty(error) == true) throw new ArgumentException($"{nameof(error)}=null");
            if (string.IsNullOrEmpty(errorDescription) == true) throw new ArgumentException($"{nameof(errorDescription)}=null");
            if (string.IsNullOrEmpty(state) == true) throw new ArgumentException($"{nameof(state)}=null");

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

            // AuthorizationToken
            if (grant_type.Equals("urn:ietf:params:oauth:grant-type:token-exchange", StringComparison.OrdinalIgnoreCase) == true)
            {
                return await this.GetTokenWithAuthorizationToken();
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
                authorizationCode = JsonSerializer.Deserialize<AuthorizationCodeData>(dataProtector.Unprotect(code), new JsonSerializerOptions
                {
                    Converters = { new ClaimsConverter() }
                });
            }
            catch (Exception exception)
            {
                return BadRequest(new { error = "invalid_grant", error_description = exception.Message });
            }
            if (authorizationCode == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode)}=null" });

            // AuthorizationCode.Require
            {
                // DataType
                if (authorizationCode.DataType.Equals(typeof(AuthorizationCodeData).FullName, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.DataType)}={authorizationCode.DataType}" });

                // ClientId
                if (authorizationCode.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ClientId)}={authorizationCode.ClientId}" });

                // CodeChallenge
                if (string.IsNullOrEmpty(authorizationCode.CodeChallenge) == true) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}" });

                // CodeChallengeMethod
                if (authorizationCode.CodeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.CodeChallengeMethod)}=null" });

                // ClaimList
                if (authorizationCode.ClaimList == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ClaimList)}=null" });

                // ExpirationTime
                if (authorizationCode.ExpirationTime <= DateTime.Now) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(authorizationCode.ExpirationTime)}={authorizationCode.ExpirationTime}" });
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

            // AuthorizationToken
            string authorizationToken = null;
            {
                // ClaimsIdentity
                var claimsIdentity = authorizationCode.GetClaimsIdentity();
                if (claimsIdentity == null) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity)}=null" });
                if (claimsIdentity.IsAuthenticated == false) return BadRequest(new { error = "invalid_grant", error_description = $"{nameof(claimsIdentity.IsAuthenticated)}=false" });

                // AuthorizationToken
                authorizationToken = dataProtector.Protect(JsonSerializer.Serialize(new AuthorizationTokenData
                (
                    client_id,
                    claimsIdentity,
                    DateTime.Now.AddMinutes(_authenticationSetting.ExpirationMinutes)
                )));
            }
            if (string.IsNullOrEmpty(authorizationToken) == true) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(authorizationToken)}=null" });

            // Return
            return Ok(new
            {
                token_type = "Bearer",
                access_token = authorizationToken,
                expires_in = _authenticationSetting.ExpirationMinutes * 60
            });
        }

        private async Task<ActionResult> GetTokenWithAuthorizationToken()
        {
            // Form
            var form = await this.Request.ReadFormAsync();
            if (form == null) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(form)}=null" });

            // Arguments
            var subject_token = form["subject_token"].ToString(); if (string.IsNullOrEmpty(subject_token) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(subject_token)}=null" });
            var subject_token_type = form["subject_token_type"].ToString(); if (string.IsNullOrEmpty(subject_token_type) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(subject_token_type)}=null" });
            var requested_token_type = form["requested_token_type"].ToString(); if (string.IsNullOrEmpty(requested_token_type) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(requested_token_type)}=null" });

            // Arguments.Require
            {
                // subject_token_type
                if (subject_token_type.Equals("urn:ietf:params:oauth:token-type:access_token", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(subject_token_type)}={subject_token_type}" });

                // requested_token_type
                if (requested_token_type.Equals("urn:ietf:params:oauth:token-type:jwt", StringComparison.OrdinalIgnoreCase) == false) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(requested_token_type)}={requested_token_type}" });
            }

            // AuthorizationTokenData
            AuthorizationTokenData authorizationTokenData = null;
            {
                // Validate
                var validateResult = this.ValidateAuthorizationToken(subject_token, out authorizationTokenData);
                if (validateResult != null) return validateResult;
            }
            if (authorizationTokenData == null) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData)}=null" });

            // AccessToken
            string accessToken = null;
            { 
                // TokenProviderFactory
                var tokenProviderFactory = this.HttpContext.RequestServices.GetService(typeof(TokenProviderFactory)) as TokenProviderFactory;
                if (tokenProviderFactory == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(tokenProviderFactory)}=null" });

                // TokenProvider
                var tokenProvider = tokenProviderFactory.CreateProvider(_authenticationSetting.JwtTokenName);
                if (tokenProvider == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(tokenProvider)}=null" });

                // ClaimsIdentity
                var claimsIdentity = authorizationTokenData.GetClaimsIdentity();
                if (claimsIdentity == null) return StatusCode(500, new { error = "invalid_token", error_description = $"{nameof(claimsIdentity)}=null" });

                // AccessToken
                accessToken = tokenProvider.CreateToken(claimsIdentity);
            }
            if (string.IsNullOrEmpty(accessToken) == true) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(accessToken)}=null" });

            // Return
            return Ok(new
            {
                token_type = "Bearer",
                issued_token_type = "urn:ietf:params:oauth:token-type:jwt",
                access_token = accessToken
            });
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/.sso/userinfo")]
        public ActionResult GetUser([FromHeader(Name = "Authorization")] string authorization)
        {
            #region Contracts

            if (string.IsNullOrEmpty(authorization) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorization)}=null" });

            #endregion

            // AuthorizationTokenData
            AuthorizationTokenData authorizationTokenData = null;
            {
                // Require
                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == false) return Unauthorized(new { error = "invalid_token", error_description = "Invalid authorization type." });

                // AuthorizationToken
                var authorizationToken = authorization.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(authorizationToken) == true) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationToken)}=null" });

                // Validate
                var validateResult = this.ValidateAuthorizationToken(authorizationToken, out authorizationTokenData);
                if (validateResult != null) return validateResult;
            }
            if (authorizationTokenData == null) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData)}=null" });

            // Return
            return this.Json(authorizationTokenData.ClaimList);
        }
    }

    public partial class AuthenticationController : Controller
    {
        // Methods
        private ActionResult ValidateAuthorizationToken(string authorizationToken, out AuthorizationTokenData authorizationTokenData)
        {
            #region Contracts

            authorizationTokenData = null;
            if (string.IsNullOrEmpty(authorizationToken) == true) return BadRequest(new { error = "invalid_request", error_description = $"{nameof(authorizationToken)}=null" });

            #endregion

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) return StatusCode(500, new { error = "server_error", error_description = $"{nameof(dataProtector)}=null" });

            // AuthorizationTokenData
            try
            {
                authorizationTokenData = JsonSerializer.Deserialize<AuthorizationTokenData>(dataProtector.Unprotect(authorizationToken), new JsonSerializerOptions
                {
                    Converters = { new ClaimsConverter() }
                });
            }
            catch (Exception exception)
            {
                return this.Unauthorized(new { error = "invalid_token", error_description = exception.Message });
            }
            if (authorizationTokenData == null) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData)}=null" });

            // AuthorizationTokenData.Require
            {
                // DataType
                if (authorizationTokenData.DataType.Equals(typeof(AuthorizationTokenData).FullName, StringComparison.OrdinalIgnoreCase) == false) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData.DataType)}={authorizationTokenData.DataType}" });

                // ClientId
                if (authorizationTokenData.ClientId.Equals("", StringComparison.OrdinalIgnoreCase) == true) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData.ClientId)}=null" });

                // ClaimList
                if (authorizationTokenData.ClaimList == null) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData.ClaimList)}=null" });

                // ExpirationTime
                if (authorizationTokenData.ExpirationTime <= DateTime.Now) this.Unauthorized(new { error = "invalid_token", error_description = $"{nameof(authorizationTokenData.ExpirationTime)}={authorizationTokenData.ExpirationTime}" });
            }

            // Return
            return null;
        }

        
        // Class
        private class AuthorizationCodeData : ClaimsBaseData
        {
            // Constructors
            public AuthorizationCodeData() : base()
            {

            }

            public AuthorizationCodeData
            (
                string clientId,
                string codeChallenge,
                string codeChallengeMethod,
                ClaimsIdentity claimsIdentity,
                DateTime expirationTime
            )
            : base(claimsIdentity, expirationTime)
            {
                #region Contracts

                if (string.IsNullOrEmpty(clientId) == true) throw new ArgumentException($"{nameof(clientId)}=null");
                if (string.IsNullOrEmpty(codeChallenge) == true) throw new ArgumentException($"{nameof(codeChallenge)}=null");
                if (string.IsNullOrEmpty(codeChallengeMethod) == true) throw new ArgumentException($"{nameof(codeChallengeMethod)}=null");
                if (claimsIdentity == null) throw new ArgumentException($"{nameof(claimsIdentity)}=null");

                #endregion

                // Default
                this.ClientId = clientId;
                this.CodeChallenge = codeChallenge;
                this.CodeChallengeMethod = codeChallengeMethod;
            }


            // Properties
            public string DataType { get; set; } = typeof(AuthorizationCodeData).FullName;

            public string ClientId { get; set; } = null;

            public string CodeChallenge { get; set; } = null;

            public string CodeChallengeMethod { get; set; } = null;
        }

        private class AuthorizationTokenData : ClaimsBaseData
        {
            // Constructors
            public AuthorizationTokenData() : base()
            {

            }

            public AuthorizationTokenData
            (
                string clientId,
                ClaimsIdentity claimsIdentity,
                DateTime expirationTime
            )
            : base(claimsIdentity, expirationTime)
            {
                #region Contracts

                if (string.IsNullOrEmpty(clientId) == true) throw new ArgumentException($"{nameof(clientId)}=null");
                if (claimsIdentity == null) throw new ArgumentException($"{nameof(claimsIdentity)}=null");

                #endregion

                // Default
                this.ClientId = clientId;
            }


            // Properties
            public string DataType { get; set; } = typeof(AuthorizationTokenData).FullName;

            public string ClientId { get; set; } = null;
        }

        private class ClaimsBaseData
        {
            // Constructors
            public ClaimsBaseData()
            {

            }

            public ClaimsBaseData(ClaimsIdentity claimsIdentity, DateTime expirationTime)
            {
                #region Contracts

                if (claimsIdentity == null) throw new ArgumentException($"{nameof(claimsIdentity)}=null");

                #endregion

                // Default
                this.ExpirationTime = expirationTime;

                // ClaimList
                this.ClaimList = claimsIdentity.Claims
                    .GroupBy(claim => claim.Type)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Count() > 1 ? (object)group.Select(claim => claim.Value).ToArray() : group.First().Value
                    );
                this.ClaimList.Remove(AuthenticationClaimTypes.AuthenticationType);
                this.ClaimList.Add(AuthenticationClaimTypes.AuthenticationType, claimsIdentity.AuthenticationType);
            }


            // Properties
            public Dictionary<string, object> ClaimList { get; set; } = null;

            public DateTime ExpirationTime { get; set; } = DateTime.MinValue;


            // Methods
            public ClaimsIdentity GetClaimsIdentity()
            {
                // ClaimList
                var claimList = new List<Claim>();
                foreach (var claim in this.ClaimList)
                {
                    // Require
                    if (string.IsNullOrEmpty(claim.Key) == true) continue;
                    if (claim.Value == null) continue;
                    if (claim.Key == AuthenticationClaimTypes.AuthenticationType) continue;

                    // Add Claim
                    if (claim.Value is string)
                    {
                        var claimValue = claim.Value as string;
                        {
                            claimList.Add(new Claim(claim.Key, claim.Value as string));
                        }
                    }

                    // Add ClaimArray
                    if (claim.Value is string[])
                    {
                        foreach (var claimValue in claim.Value as string[])
                        {
                            claimList.Add(new Claim(claim.Key, claimValue));
                        }
                    }
                }

                // AuthenticationType
                var authenticationType = this.ClaimList.FirstOrDefault(o => o.Key == AuthenticationClaimTypes.AuthenticationType).Value as string;
                if (string.IsNullOrEmpty(authenticationType) == true) throw new InvalidOperationException($"{nameof(authenticationType)}=null");

                // Return
                return new ClaimsIdentity(claimList, authenticationType);
            }
        }

        private class ClaimsConverter : JsonConverter<object>
        {
            // Methods
            public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                #region Contracts

                if (typeToConvert == null) throw new ArgumentException($"{nameof(typeToConvert)}=null");
                if (options == null) throw new ArgumentException($"{nameof(options)}=null");

                #endregion

                // String
                if (reader.TokenType == JsonTokenType.String) return reader.GetString();

                // StringArray
                if (reader.TokenType == JsonTokenType.StartArray) return JsonSerializer.Deserialize<string[]>(ref reader, options);

                // Object
                return JsonSerializer.Deserialize<object>(ref reader, options);
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                #region Contracts

                if (writer == null) throw new ArgumentException($"{nameof(writer)}=null");
                if (options == null) throw new ArgumentException($"{nameof(options)}=null");

                #endregion

                // Write
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }
    }
}
