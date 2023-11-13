﻿using Microsoft.AspNetCore.Http;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public class AzureServicesAuthenticationSelector : PolicyAuthenticationSelector
    {
        // Fields
        private readonly string _scheme = String.Empty;

        private readonly string _header = String.Empty;

        private readonly string _prefix = String.Empty;


        // Constructors
        public AzureServicesAuthenticationSelector(string scheme, string header, string prefix = null)
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
