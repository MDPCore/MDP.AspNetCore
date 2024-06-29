using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Tokens
{
    public class TokenAuthenticationSelector : PolicyAuthenticationSelector
    {
        // Fields
        private readonly string _scheme = String.Empty;

        private readonly string _header = String.Empty;

        private readonly string _prefix = String.Empty;


        // Constructors
        public TokenAuthenticationSelector(string scheme, string header, string prefix = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentNullException($"{nameof(header)}=null");

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

            if (context == null) throw new ArgumentNullException($"{nameof(context)}=null");

            #endregion

            // Authorization
            string authorization = context.Request.Headers[_header];
            if (string.IsNullOrEmpty(authorization) == true) return false;

            // Prefix
            if (string.IsNullOrEmpty(_prefix) == true)
            {
                return true;
            }
            if (string.IsNullOrEmpty(_prefix) == false && authorization.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            // Return
            return false;
        }
    }
}
