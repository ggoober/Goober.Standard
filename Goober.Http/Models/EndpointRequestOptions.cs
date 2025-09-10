using Goober.Http.Models.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Goober.Http.Models
{
    public class EndpointRequestOptions
    {
        public Func<AuthenticateContext, Task<AuthenticateResponse>> AuthenticateAsync { get; set; }
        public Func<HttpRequestContext, Task<HttpRequestMessage>> BeforeHttpRequestSendAsync { get; set; }
        public Func<HttpResponseContext, Task<HttpResponseMessage>> AfterHttpResponseRecivedAsync { get; set; }
    }
}
