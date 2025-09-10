using System;
using System.Net.Http;

namespace Goober.Http.Models.Internal
{
    public class HttpRequestContext
    {
        public IServiceProvider ServiceProvider { get; set; }
        public HttpRequestMessage HttpRequestMessage { get; set; }
    }
}
