using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public class PathDefaultRule : IRule
    {
        // Constants
        private static readonly PathString _pathRoot = new PathString("/");


        // Fields
        private readonly PathString _pathDefault = PathString.Empty;


        // Constructors
        public PathDefaultRule(string pathDefault)
        {
            #region Contracts

            if (string.IsNullOrEmpty(pathDefault) == true) throw new ArgumentException($"{nameof(pathDefault)}=null");

            #endregion

            // Default
            _pathDefault = new PathString(pathDefault);

            // Require
            if (_pathDefault == _pathRoot) throw new InvalidOperationException($"{nameof(_pathDefault)}={_pathRoot.Value}");
        }


        // Methods
        public void ApplyRule(RewriteContext context)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");

            #endregion
                        
            // Path
            var path = context.HttpContext.Request.Path;
            if (path == _pathRoot) path = PathString.Empty;
            if (path.HasValue == true) return;

            // NewPath
            var newPath = _pathDefault + context.HttpContext.Request.QueryString.ToUriComponent();

            // Redirect
            context.HttpContext.Response.StatusCode = StatusCodes.Status302Found;
            context.HttpContext.Response.Headers[HeaderNames.Location] = newPath;
            context.Result = RuleResult.EndResponse;
        }
    }
}
