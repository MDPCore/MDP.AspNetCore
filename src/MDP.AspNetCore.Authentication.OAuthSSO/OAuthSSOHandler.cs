using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MDP.AspNetCore.Authentication.OAuthSSO
{
    public class OAuthSSOHandler : OAuthHandler<OAuthSSOOptions>
    {
        // Constructors
        public OAuthSSOHandler(IOptionsMonitor<OAuthSSOOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }


        // Methods
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            #region Contracts

            if (identity == null) throw new ArgumentException(nameof(identity));
            if (properties == null) throw new ArgumentException(nameof(properties));
            if (tokens == null) throw new ArgumentException(nameof(tokens));

            #endregion

            // Request
            var request = new HttpRequestMessage(HttpMethod.Post, this.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            // Response
            var response = await this.Backchannel.SendAsync(request, this.Context.RequestAborted);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // UserInfoData
            UserInfoData userInfoData = null;
            try
            {
                userInfoData = JsonSerializer.Deserialize<UserInfoData>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
                {
                    Converters = { new ClaimsConverter() }
                });
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
            if (userInfoData == null) throw new InvalidOperationException($"{nameof(userInfoData)}=null");

            // ClaimsIdentity
            identity = userInfoData.GetClaimsIdentity();
            if (identity == null) throw new InvalidOperationException($"{nameof(identity)}=null");

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // CreatingTicketContext
                var creatingTicketContext = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, payload.RootElement);
                creatingTicketContext.RunClaimActions();
                await Events.CreatingTicket(creatingTicketContext);

                // AuthenticationTicket
                var authenticationTicket = new AuthenticationTicket(creatingTicketContext.Principal!, creatingTicketContext.Properties, this.Scheme.Name);

                // Return
                return authenticationTicket;
            }
        }


        // Class
        private class UserInfoData : Dictionary<string, object>
        {
            // Methods
            public ClaimsIdentity GetClaimsIdentity()
            {
                // ClaimList
                var claimList = new List<Claim>();
                foreach (var claim in this)
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
                var authenticationType = this.FirstOrDefault(o => o.Key == AuthenticationClaimTypes.AuthenticationType).Value as string;
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
