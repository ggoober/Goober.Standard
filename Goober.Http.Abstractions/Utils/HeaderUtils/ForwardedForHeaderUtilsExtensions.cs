using System.Collections.Generic;
using System.Linq;
using Goober.Http.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Goober.Http.Utils.HeaderUtils
{
    internal static class ForwardedForHeaderUtilsExtensions
    {
        internal static void SetXForwardedForHeader(
            this List<KeyValuePair<string, string>> headerValues,
            HttpContext httpContext)
        {
            if (httpContext == null)
                return;

            if (headerValues == null)
            {
                headerValues = new List<KeyValuePair<string, string>>();
            }
            
            var isXOriginalFor = httpContext.Request.Headers.TryGetValue(HttpGlossary.X_ORIGINAL_FOR, out var xOriginalFor);
            var isXForwardedFor = httpContext.Request.Headers.TryGetValue(HttpGlossary.X_FORWARDED_FOR, out var xForwardedFor);
            
            // Not middleware or Not proxy
            if (isXOriginalFor != true && isXForwardedFor != true)
            {
                bool isHeaderValue = TryGetXForwardedForWithoutMiddlewareAndProxy(httpContext, out var headerValue);
                if (isHeaderValue == true)
                {
                    headerValues.Add(new KeyValuePair<string, string>(HttpGlossary.X_FORWARDED_FOR, headerValue));
                }
                return;
            }

            // Exist middleware and proxy
            if (isXOriginalFor == true && isXForwardedFor == true)
            {
                string headerValue = GetXForwardedForWithMiddlewareAndProxy(
                    httpContext: httpContext,
                    xOriginFor: xOriginalFor,
                    xForwardedFor: xForwardedFor);
                if (string.IsNullOrEmpty(headerValue) == false)
                {
                    headerValues.Add(new KeyValuePair<string, string>(HttpGlossary.X_FORWARDED_FOR, headerValue));
                }
                return;
            }

            // Not middleware
            if (isXOriginalFor != true)
            {
                var headerValue = GetXForwardedForWithoutMiddleware(
                    httpContext: httpContext,
                    xForwardedFor: xForwardedFor);
                if (string.IsNullOrEmpty(headerValue) == false)
                {
                    headerValues.Add(new KeyValuePair<string, string>(HttpGlossary.X_FORWARDED_FOR, headerValue));
                }
                return;
            }
            
            bool isValue = TryGetXForwardedForWithoutProxyOrFirst(
                httpContext: httpContext,
                xOriginFor: xOriginalFor,
                headerValue: out var value);
                
            if (isValue == true)
            {
                headerValues.Add(new KeyValuePair<string, string>(HttpGlossary.X_FORWARDED_FOR, value));
            }
        }

        private static bool TryGetXForwardedForWithoutProxyOrFirst(
            HttpContext httpContext,
            string xOriginFor,
            out string headerValue)
        {
            headerValue = null;
            
            var remoteIpAddress = GetRemoteIpAddressAndPort(httpContext);
            var localIpAddress = GetLocalIpAddressAndPort(httpContext);

            var sequence = new string[]
            {
                remoteIpAddress,
                xOriginFor,
                localIpAddress
            };

            var values = sequence
                .Where(x => string.IsNullOrEmpty(x) == false)
                .ToArray();

            if (values.Length == 0)
                return false;

            headerValue = string.Join(", ", values);
            return true;
        }

        private static string GetXForwardedForWithoutMiddleware(
            HttpContext httpContext,
            string xForwardedFor)
        {
            var localIpAddress = GetLocalIpAddressAndPort(httpContext);
            
            if (string.IsNullOrEmpty(localIpAddress) == false)
            {
                return $"{xForwardedFor}, {localIpAddress}";
            }
            
            return xForwardedFor;
        }

        private static string GetXForwardedForWithMiddlewareAndProxy(
            HttpContext httpContext,
            string xOriginFor,
            string xForwardedFor)
        {
            var localIpAddress = GetLocalIpAddressAndPort(httpContext);
            
            if (string.IsNullOrEmpty(localIpAddress) == false)
            {
                return $"{xForwardedFor}, {xOriginFor}, {localIpAddress}";
            }
            
            return $"{xForwardedFor}, {xOriginFor}";
        }

        private static bool TryGetXForwardedForWithoutMiddlewareAndProxy(
            HttpContext httpContext,
            out string headerValue)
        {
            headerValue = null;
            
            var remoteIpAddress = GetRemoteIpAddressAndPort(httpContext);
            var localIpAddress = GetLocalIpAddressAndPort(httpContext);
            var isRemoteIpAddress = string.IsNullOrEmpty(remoteIpAddress) == false;
            var isLocalIpAddress = string.IsNullOrEmpty(localIpAddress) == false;

            if (isRemoteIpAddress && isLocalIpAddress)
            {
                headerValue = $"{remoteIpAddress}, {localIpAddress}";
                return true;
            }

            if (isRemoteIpAddress)
            {
                headerValue = remoteIpAddress;
                return true;
            }

            if (isLocalIpAddress)
            {
                headerValue = localIpAddress;
                return true;
            }

            return false;
        }

        private static string GetRemoteIpAddressAndPort(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;
            
            var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var remotePort = httpContext.Connection.RemotePort;
            
            if (string.IsNullOrEmpty(remoteIpAddress) == true)
            {
                return null;
            }

            if (remotePort != 0)
            {
                return $"{remoteIpAddress}:{remotePort}";
            }

            return remoteIpAddress;
        }
        
        private static string GetLocalIpAddressAndPort(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;
            
            var ipAddress = httpContext.Connection.LocalIpAddress?.ToString();
            var port = httpContext.Connection.LocalPort;
            
            if (string.IsNullOrEmpty(ipAddress) == true)
            {
                return null;
            }

            if (port != 0)
            {
                return $"{ipAddress}:{port}";
            }

            return ipAddress;
        }
    }
}