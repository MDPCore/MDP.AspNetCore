#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public class PolicyAuthenticationSetting
    {
        // Properties
        public string DefaultScheme { get; set; } = String.Empty;
    }
}
