using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public class SecurityTokenPolicyAuthenticationSelector : PolicyAuthenticationSelector
    {
        // Fields
        private readonly string _scheme = String.Empty;

        private readonly string _header = String.Empty;

        private readonly string _prefix = String.Empty;


        // Constructors
        public SecurityTokenPolicyAuthenticationSelector(string scheme, string header, string prefix)
        {
            #region Contracts

            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentException($"{nameof(header)}=null");

            #endregion

            // Default
            _scheme = scheme;
            _header = header;
            _prefix = prefix;
        }

        // Properties
        public string AuthenticationScheme { get { return _scheme; } }


        // Methods
        public bool Check(HttpContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");

            #endregion

            // Authorization
            string authorization = context.Request.Headers[_header];
            if (string.IsNullOrEmpty(authorization) == true) return false;

            // Token
            string token = string.Empty;
            if (string.IsNullOrEmpty(_prefix) == true)
            {
                token = authorization;
            }
            if (string.IsNullOrEmpty(_prefix) == false && authorization.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase) == true)
            {
                token = authorization.Substring(_prefix.Length).Trim();
            }
            if (string.IsNullOrEmpty(token) == true) return false;

            // Return
            return true;
        }
    }
}
