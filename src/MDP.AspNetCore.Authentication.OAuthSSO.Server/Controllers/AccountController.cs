using MDP.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public partial class AccountController : Controller
    {
        // Fields
        private readonly IList<ClientCredential> _clientCredentialList = null;

        private readonly IDataProtectionProvider _dataProtectionProvider = null;

        private readonly int _expirationMinutes = 5;


        // Constructors
        public AccountController(IList<ClientCredential> clientCredentialList, IDataProtectionProvider dataProtectionProvider)
        {
            #region Contracts

            if (clientCredentialList == null) throw new ArgumentException($"{nameof(clientCredentialList)}=null");
            if (dataProtectionProvider == null) throw new ArgumentException($"{nameof(dataProtectionProvider)}=null");

            #endregion

            // Default
            _clientCredentialList = clientCredentialList;
            _dataProtectionProvider = dataProtectionProvider;
        }


        // Methods
        [Authorize]
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

            if (string.IsNullOrEmpty(client_id) == true) throw new ArgumentException($"{nameof(client_id)}=null");
            if (string.IsNullOrEmpty(redirect_uri) == true) throw new ArgumentException($"{nameof(redirect_uri)}=null");
            if (string.IsNullOrEmpty(response_type) == true) throw new ArgumentException($"{nameof(response_type)}=null");
            if (string.IsNullOrEmpty(code_challenge) == true) throw new ArgumentException($"{nameof(code_challenge)}=null");
            if (string.IsNullOrEmpty(code_challenge_method) == true) throw new ArgumentException($"{nameof(code_challenge_method)}=null");
            if (string.IsNullOrEmpty(state) == true) throw new ArgumentException($"{nameof(state)}=null");

            #endregion

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) throw new InvalidOperationException($"{nameof(dataProtector)}=null");

            // ClientCredential
            var clientCredential = _clientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) throw new InvalidOperationException($"{nameof(clientCredential)}=null");

            // ClientCredential.Require
            {
                // client_id
                if (client_id.Equals(clientCredential.ClientId, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(client_id)}={client_id}");

                // redirect_uri
                if (redirect_uri.StartsWith(clientCredential.RedirectUri, StringComparison.OrdinalIgnoreCase)==false) return this.Unauthorized($"{nameof(redirect_uri)}={redirect_uri}");

                // response_type
                if (response_type.Equals("code", StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(response_type)}={response_type}");

                // code_challenge

                // code_challenge_method
                if (code_challenge_method.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(code_challenge_method)}={code_challenge_method}");

                // state
            }

            // AuthorizationCode
            string authorizationCode = null;
            {                
                // ClaimsIdentity
                var claimsIdentity = this.User?.Identity as ClaimsIdentity;
                if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");
                if (claimsIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(claimsIdentity.IsAuthenticated)}=false");

                // AuthorizationCode
                authorizationCode = dataProtector.Protect(JsonSerializer.Serialize(new AuthorizationCodeData
                (
                    client_id,
                    code_challenge,
                    code_challenge_method,
                    claimsIdentity,
                    DateTime.Now.AddMinutes(_expirationMinutes)
                )));
            }
            if (string.IsNullOrEmpty(authorizationCode) == true) throw new InvalidOperationException($"{nameof(authorizationCode)}=null");

            // Return
            return this.Redirect($"{redirect_uri}?code={Uri.EscapeDataString(authorizationCode)}&state={Uri.EscapeDataString(state)}");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/.sso/token")]
        public ActionResult GetToken
        (
            string client_id,
            string redirect_uri,
            string grant_type,
            string code,
            string code_verifier
        )
        {
            #region Contracts

            if (string.IsNullOrEmpty(client_id) == true) throw new ArgumentException($"{nameof(client_id)}=null");
            if (string.IsNullOrEmpty(redirect_uri) == true) throw new ArgumentException($"{nameof(redirect_uri)}=null");
            if (string.IsNullOrEmpty(grant_type) == true) throw new ArgumentException($"{nameof(grant_type)}=null");
            if (string.IsNullOrEmpty(code) == true) throw new ArgumentException($"{nameof(code)}=null");
            if (string.IsNullOrEmpty(code_verifier) == true) throw new ArgumentException($"{nameof(code_verifier)}=null");

            #endregion

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) throw new InvalidOperationException($"{nameof(dataProtector)}=null");

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
                return this.Unauthorized(exception.Message);
            }
            if (authorizationCode == null) return this.Unauthorized($"{nameof(authorizationCode)}=null");

            // AuthorizationCode.Require
            {
                // DataType
                if (authorizationCode.DataType.Equals(typeof(AuthorizationCodeData).FullName, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(authorizationCode.DataType)}={authorizationCode.DataType}");

                // ClientId
                if (authorizationCode.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(authorizationCode.ClientId)}={authorizationCode.ClientId}");

                // CodeChallenge
                if (string.IsNullOrEmpty(authorizationCode.CodeChallenge) == true) return this.Unauthorized($"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}");

                // CodeChallengeMethod
                if (authorizationCode.CodeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(authorizationCode.CodeChallengeMethod)}=null");

                // ClaimList
                if (authorizationCode.ClaimList == null) return this.Unauthorized($"{nameof(authorizationCode.ClaimList)}=null");

                // ExpirationTime
                if (authorizationCode.ExpirationTime <= DateTime.Now) return this.Unauthorized($"{nameof(authorizationCode.ExpirationTime)}={authorizationCode.ExpirationTime}");
            }

            // ClientCredential
            var clientCredential = _clientCredentialList.First(o => o.ClientId.Equals(client_id, StringComparison.OrdinalIgnoreCase));
            if (clientCredential == null) throw new InvalidOperationException($"{nameof(clientCredential)}=null");

            // ClientCredential.Require
            {
                // client_id
                if (client_id.Equals(clientCredential.ClientId, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(client_id)}={client_id}");

                // redirect_uri
                if (redirect_uri.StartsWith(clientCredential.RedirectUri, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(redirect_uri)}={redirect_uri}");

                // grant_type
                if (grant_type.Equals("authorization_code", StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(grant_type)}={grant_type}");

                // code

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
                if (string.IsNullOrEmpty(codeVerifier) == true) throw new InvalidOperationException($"{nameof(codeVerifier)}=null");
                if (codeVerifier.Equals(authorizationCode.CodeChallenge) == false) return this.Unauthorized($"{nameof(authorizationCode.CodeChallenge)}={authorizationCode.CodeChallenge}");
            }

            // AccessToken
            string accessToken = null;
            {
                // ClaimsIdentity
                var claimsIdentity = authorizationCode.GetClaimsIdentity();
                if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");
                if (claimsIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(claimsIdentity.IsAuthenticated)}=false");

                // AccessToken
                accessToken = dataProtector.Protect(JsonSerializer.Serialize(new AccessTokenData
                (
                    client_id,
                    claimsIdentity,
                    DateTime.Now.AddMinutes(_expirationMinutes)
                )));
            }
            if (string.IsNullOrEmpty(accessToken) == true) throw new InvalidOperationException($"{nameof(accessToken)}=null");

            // Return
            return Ok(new
            {
                token_type = "Bearer",
                access_token = accessToken,
                expires_in = _expirationMinutes * 60
            });
        }

        [AllowAnonymous]
        [Route("/.sso/userinfo")]
        public ActionResult GetUser([FromHeader(Name = "Authorization")] string authorization)
        {
            #region Contracts

            if (string.IsNullOrEmpty(authorization) == true) throw new ArgumentException($"{nameof(authorization)}=null");

            #endregion

            // Require
            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)==false) return Unauthorized("Invalid authorization type.");

            // DataProtector
            var dataProtector = _dataProtectionProvider.CreateProtector(this.GetType().FullName);
            if (dataProtector == null) throw new InvalidOperationException($"{nameof(dataProtector)}=null");

            // AccessTokenData
            AccessTokenData accessTokenData = null;
            try
            {
                accessTokenData = JsonSerializer.Deserialize<AccessTokenData>(dataProtector.Unprotect(authorization.Substring("Bearer ".Length).Trim()), new JsonSerializerOptions
                {
                    Converters = { new ClaimsConverter() }
                });
            }
            catch (Exception exception)
            {
                return this.Unauthorized(exception.Message);
            }
            if (accessTokenData == null) return this.Unauthorized($"{nameof(accessTokenData)}=null");

            // AccessTokenData.Require
            {
                // DataType
                if (accessTokenData.DataType.Equals(typeof(AccessTokenData).FullName, StringComparison.OrdinalIgnoreCase) == false) return this.Unauthorized($"{nameof(accessTokenData.DataType)}={accessTokenData.DataType}");

                // ClientId
               
                // ClaimList
                if (accessTokenData.ClaimList == null) return this.Unauthorized($"{nameof(accessTokenData.ClaimList)}=null");

                // ExpirationTime
                if (accessTokenData.ExpirationTime <= DateTime.Now) return this.Unauthorized($"{nameof(accessTokenData.ExpirationTime)}={accessTokenData.ExpirationTime}");
            }

            // Return
            return this.Json(accessTokenData.ClaimList);
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

        private class AccessTokenData : ClaimsBaseData
        {
            // Constructors
            public AccessTokenData() : base()
            {

            }

            public AccessTokenData
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
            public string DataType { get; set; } = typeof(AccessTokenData).FullName;

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
