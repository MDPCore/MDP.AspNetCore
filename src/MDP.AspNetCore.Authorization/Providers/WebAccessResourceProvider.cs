using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class WebAccessResourceProvider : IAccessResourceProvider
    {
        // Methods
        public AccessResource Create(object resource = null)
        {
            // HttpContext
            var httpContext = resource as HttpContext;
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
