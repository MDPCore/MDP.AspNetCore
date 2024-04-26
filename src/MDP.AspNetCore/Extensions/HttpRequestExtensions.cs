using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public static class HttpRequestExtensions
    {
        // Methods
        public static bool IsAPI(this HttpRequest request)
        {
            #region Contracts

            if (request == null) throw new ArgumentException($"{nameof(request)}=null");

            #endregion

            // Return
            return (request.HasAccept("html") == false);
        }

        private static bool HasAccept(this HttpRequest request, string accept)
        {
            #region Contracts

            if (request == null) throw new ArgumentException($"{nameof(request)}=null");
            if (string.IsNullOrEmpty(accept) == true) throw new ArgumentException($"{nameof(accept)}=null");

            #endregion

            // Return
            return request.HasAccept(new List<string>() { accept });
        }

        private static bool HasAccept(this HttpRequest request, List<string> acceptList) 
        {
            #region Contracts

            if (request == null) throw new ArgumentException($"{nameof(request)}=null");
            if (acceptList == null) throw new ArgumentException($"{nameof(acceptList)}=null");

            #endregion

            // AcceptList
            foreach (var accept in acceptList)
            {
                // Require
                if (string.IsNullOrEmpty(accept)==true) throw new InvalidProgramException($"{nameof(accept)}=true");

                // Contains
                if (request.Headers.Accept.Any(o => o.Contains(accept, StringComparison.OrdinalIgnoreCase)) == true) return true;
            }

            // Return
            return false;
        }
    }
}
