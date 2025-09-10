using System;

namespace Goober.Http.Models.Internal
{
    public class AuthenticateContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public string EndPointSchemeAndHost { get; set; }

        public string EndPointAlias { get; set; }

        public string RequestUrl { get; set; }
    }
}
