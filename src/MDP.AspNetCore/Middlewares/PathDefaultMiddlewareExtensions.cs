using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public static class PathDefaultMiddlewareExtensions
    {
        // Methods        
        public static WebApplication UsePathDefault(this WebApplication host, string pathDefault = "/Home/Index")
        {
            #region Contracts

            if (host == null) throw new ArgumentException($"{nameof(host)}=null");
            if (string.IsNullOrEmpty(pathDefault) == true) throw new ArgumentException($"{nameof(pathDefault)}=null");

            #endregion

            // Rewriter
            host.UseRewriter(new RewriteOptions()

                // PathDefaultRule
                .Add(new PathDefaultRule(pathDefault))
            );

            // Return
            return host;
        }
    }
}
