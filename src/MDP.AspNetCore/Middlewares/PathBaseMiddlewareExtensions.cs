using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public static class PathBaseMiddlewareExtensions
    {
        // Methods        
        public static WebApplication UsePathBase(this WebApplication host)
        {
            #region Contracts

            if (host == null) throw new ArgumentException($"{nameof(host)}=null");

            #endregion

            // UsePathBase
            //host.UsePathBase(new PathString("/AAA"));

            // Return
            return host;
        }
    }
}
