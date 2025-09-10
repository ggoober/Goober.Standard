using System.Collections.Generic;
using Goober.Http.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Goober.Http.Utils.HeaderUtils
{
    internal static class ForwardedHostHeaderUtilsExtensions
    {
        internal static void SetXForwardedHostHeader(
            this List<KeyValuePair<string, string>> headerValues,
            HttpContext httpContext)
        {
            if (httpContext == null)
                return;

            if (headerValues == null)
            {
                headerValues = new List<KeyValuePair<string, string>>();
            }

            var requestHost = httpContext.Request.Host.Value;
            if (string.IsNullOrEmpty(requestHost) == false)
            {
                headerValues.Add(new KeyValuePair<string, string>(HttpGlossary.X_FORWARDED_HOST, requestHost));
            }
        }
    }
}