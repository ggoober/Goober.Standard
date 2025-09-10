using System;
using System.Net.Http;

namespace Goober.Http.Models.Internal
{
    public class HttpResponseContext
    {
        public IServiceProvider ServiceProvider { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
    }
}
