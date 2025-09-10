using Microsoft.AspNetCore.Http;

namespace Goober.Http.Abstractions
{
    internal static class ApplicationContextSingleton
    {
        private const string ContextApplicationKey = "APPLICATION";
        private const string ContextEnvironmentKey = "ENVIRONMENT";
        private const string ContextCustomerKey = "CUSTOMER";
        private const string ContextVersionKey = "VERSION";

        private class StringContainer
        {
            public string Value { get; set; }
        }

        private static StringContainer _application;

        public static string GetApplication(HttpContext httpContext)
        {
            if (_application == null)
            {
                var ret = httpContext?.Items?.ContainsKey(ContextApplicationKey) == true ? httpContext?.Items[ContextApplicationKey]?.ToString() : null;

                _application = new StringContainer { Value = ret };
            }

            return _application.Value;
        }

        private static StringContainer _customer;

        public static string GetCustomer(HttpContext httpContext)
        {
            if (_customer == null)
            {
                var ret = httpContext?.Items?.ContainsKey(ContextCustomerKey) == true ? httpContext?.Items[ContextCustomerKey]?.ToString() : null;

                _customer = new StringContainer { Value = ret };
            }

            return _customer.Value;
        }

        private static StringContainer _environment;

        public static string GetEnvironment(HttpContext httpContext)
        {
            if (_environment == null)
            {
                var ret = httpContext?.Items?.ContainsKey(ContextEnvironmentKey) == true ? httpContext?.Items[ContextEnvironmentKey]?.ToString() : null;

                _environment = new StringContainer { Value = ret };
            }

            return _environment.Value;
        }

        private static StringContainer _version;

        public static string GetVersion(HttpContext httpContext)
        {
            if (_version == null)
            {
                var ret = httpContext?.Items?.ContainsKey(ContextVersionKey) == true ? httpContext?.Items[ContextVersionKey]?.ToString() : null;

                _version = new StringContainer { Value = ret };
            }

            return _version.Value;
        }
    }
}
