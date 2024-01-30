using MDP.AspNetCore.Authentication.Line;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MDP.AspNetCore.Authentication.Liff
{
    public class LiffOptions : LineOptions
    {
        // Constructors
        public LiffOptions()
        {
            // Options
            this.ChallengeUrl = new PathString("/.auth/login/liff");
            this.CallbackPath = new PathString("/.auth/login/liff/callback");
            this.ClaimsIssuer = LiffDefaults.ClaimsIssuer;
        }


        // Properties
        public string LiffId { get; set; } = string.Empty;

        public string ChallengeUrl { get; set; } = string.Empty;
    }
}
