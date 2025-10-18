using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineHandler : LineHandler<LineOptions>
    {
        // Constructors
        public LineHandler(IOptionsMonitor<LineOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }
    }

    public class LineHandler<TOptions> : OAuthHandler<TOptions>
        where TOptions : LineOptions, new()
    {
        // Constructors
        public LineHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }


        // Methods
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            #region Contracts

            if (identity == null) throw new ArgumentNullException(nameof(identity));
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));

            #endregion

            // IdToken
            var idToken = tokens.Response.RootElement.GetProperty("id_token").ToString();
            if (idToken == "null") idToken = string.Empty;
            if (string.IsNullOrEmpty(idToken) == true) throw new InvalidOperationException($"{nameof(idToken)} is emptied. Please check openid scopes is enable.");

            // Friendship
            if (this.Options.RequireFriendship == true)
            {
                var friendship = await this.GetFriendshipAsync(tokens.AccessToken);
                if (friendship == false)
                {
                    if (identity.HasClaim(claim => claim.Type == "mdp:line:friend") == false)
                    {
                        identity.AddClaim(new Claim("mdp:line:friend", "false"));
                    }
                }
            }

            // Request
            var request = new HttpRequestMessage(HttpMethod.Post, this.Options.UserInformationEndpoint);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", this.Options.ClientId},
                { "id_token", idToken}
            });

            // Response
            var response = await this.Backchannel.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

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

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            #region Contracts

            if (properties == null) throw new ArgumentNullException(nameof(properties));
            //if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentNullException(nameof(redirectUri));

            #endregion

            // Require
            redirectUri = string.IsNullOrEmpty(redirectUri) ? "/" : redirectUri;
            properties.RedirectUri = string.IsNullOrEmpty(properties.RedirectUri) ? "/" : properties.RedirectUri;

            // Friendship
            var promptList = new Dictionary<string, string>();
            if(this.Options.RequireFriendship == true)
            {
                if (properties.RedirectUri.StartsWith("/.auth/login/line/check", StringComparison.OrdinalIgnoreCase) == false)
                {
                    // Add
                    promptList.Add("bot_prompt", "aggressive");

                    // ReturnUrl
                    var returnUrl = QueryHelpers.AddQueryString("/.auth/login/line/check", "returnUrl", properties.RedirectUri);
                    if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");

                    // RedirectUri
                    properties.RedirectUri = returnUrl;
                }
                else
                {
                    // Add
                    promptList.Add("prompt", "consent");
                    promptList.Add("bot_prompt", "aggressive");

                    // ReturnUrl
                    var returnUrl = properties.RedirectUri;
                    if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");

                    // RedirectUri
                    properties.RedirectUri = returnUrl;
                }
            }            

            // ChallengeUrl
            var challengeUrl = base.BuildChallengeUrl(properties, redirectUri);
            if (string.IsNullOrEmpty(challengeUrl) == true) throw new InvalidOperationException($"{nameof(challengeUrl)}=null");
            if (string.IsNullOrEmpty(challengeUrl) == false)
            {
                foreach(var prompt in promptList)
                {
                    // Add
                    challengeUrl = QueryHelpers.AddQueryString(challengeUrl, prompt.Key, prompt.Value);
                    if (string.IsNullOrEmpty(challengeUrl) == true) throw new InvalidOperationException($"{nameof(challengeUrl)}=null"); ;
                }                
            }

            // Return
            return challengeUrl;
        }

        protected virtual async Task<bool> GetFriendshipAsync(string accessToken)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessToken) == true) throw new ArgumentNullException(nameof(accessToken));

            #endregion

            // Request
            var request = new HttpRequestMessage(HttpMethod.Get, this.Options.FriendshipEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Response
            var response = await this.Backchannel.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving friendship status ({response.StatusCode}).");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                if (payload.RootElement.TryGetProperty("friendFlag", out var friendFlag))
                {
                    if (friendFlag.ValueKind == JsonValueKind.True) return true;
                }
            }

            // Return
            return false;
        }
    }
}
