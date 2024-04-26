using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    internal class HookMiddleware
    {
        // Fields
        private readonly string _hookName;

        private readonly Action<WebApplication> _configureMiddleware;


        // Constructors
        public HookMiddleware(string hookName, Action<WebApplication> configureMiddleware)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hookName) == true) throw new ArgumentException($"{nameof(hookName)}=null");
            if (configureMiddleware == null) throw new ArgumentException($"{nameof(configureMiddleware)}=null");

            #endregion

            // Default
            _hookName = hookName;
            _configureMiddleware = configureMiddleware;
        }


        // Properties
        public string HookName { get { return _hookName; } }


        // Methods
        public void ConfigureMiddleware(WebApplication host)
        {
            #region Contracts

            if (host == null) throw new ArgumentException($"{nameof(host)}=null");

            #endregion

            // Action
            _configureMiddleware?.Invoke(host);
        }
    }
}
