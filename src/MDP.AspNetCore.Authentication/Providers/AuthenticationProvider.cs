using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public interface AuthenticationProvider
    {
        // Properties
        string RegisterPath { get; set; }


        // Methods
        ClaimsIdentity SignIn(ClaimsIdentity remoteIdentity);
    }

    internal class DefaultAuthenticationProvider : AuthenticationProvider
    {
        // Singleton 
        private static DefaultAuthenticationProvider _instance = null;
        public static DefaultAuthenticationProvider Current
        {
            get
            {
                // Create
                if (_instance == null)
                {
                    _instance = new DefaultAuthenticationProvider();
                }

                // Return
                return _instance;
            }
        }


        // Properties
        public string RegisterPath { get; set; } = string.Empty;


        // Methods
        public ClaimsIdentity SignIn(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Return
            return remoteIdentity;
        }
    }
}
