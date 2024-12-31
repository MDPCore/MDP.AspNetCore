using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Apple
{
    public class AppleDefaults
    {
        // Constants
        public static readonly string AuthenticationScheme = "Apple";

        public static readonly string AuthorizationEndpoint = "https://appleid.apple.com/auth/authorize";

        public static readonly string TokenEndpoint = "https://appleid.apple.com/auth/token";
    }
}