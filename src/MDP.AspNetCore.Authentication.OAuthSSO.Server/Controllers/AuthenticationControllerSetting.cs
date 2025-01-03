﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class AuthenticationControllerSetting
    {
        // Properties
        public string JwtTokenName { get; set; } = "JwtBearer";

        public int AuthorizationCodeExpireMinutes { get; set; } = 1; // 1 minutes

        public int AccessTokenExpireMinutes { get; set; } = 30; // 30 minutes

        public int RefreshTokenExpireMinutes { get; set; } = 20160; // 14 days

        public List<ClientCredential> ClientCredentialList { get; set; } = null;
    }
}
