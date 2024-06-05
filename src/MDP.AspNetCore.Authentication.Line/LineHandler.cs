using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineHandler : LineHandler<LineOptions>
    {
        // Constructors
        public LineHandler(IOptionsMonitor<LineOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
    }

    public class LineHandler<TOptions> : OAuthHandler<TOptions>
        where TOptions : LineOptions, new()
    {
        // Constructors
        public LineHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }


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
    }
}
