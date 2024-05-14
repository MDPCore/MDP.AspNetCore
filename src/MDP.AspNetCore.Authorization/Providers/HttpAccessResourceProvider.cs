using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class HttpAccessResourceProvider : IAccessResourceProvider
    {
        // Fields
        private readonly AuthorizationHandlerContext _authorizationHandlerContext = null;


        // Constructors
        public HttpAccessResourceProvider(AuthorizationHandlerContext authorizationHandlerContext)
        {
            #region Contracts

            if (authorizationHandlerContext == null) throw new ArgumentException($"{nameof(authorizationHandlerContext)}=null");

            #endregion

            // Default
            _authorizationHandlerContext = authorizationHandlerContext;
        }


        // Methods
        public AccessResource Create()
        {
            // HttpContext
            var httpContext = _authorizationHandlerContext.Resource as HttpContext;
            if (httpContext == null) return null;

            // Request
            var request = httpContext.Request;
            if (request == null) return null;

            // AccessResource
            var accessResource = new AccessResource($"{request.Scheme}://{request.Host}{request.Path}");
            if (accessResource == null) return null;

            // Return
            return accessResource;
        }
    }
}
