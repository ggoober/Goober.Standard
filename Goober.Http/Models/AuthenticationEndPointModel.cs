using Goober.Http.Models.Internal;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Goober.Http.Models
{
    public class AuthenticationEndPointModel
    {
        public string EndPointAlias { get; set; }
        public string EndPointSchemeAndHost { get; set; }
        public int RetryCount { get; set; } = 1;
        public TimeSpan RetryPeriod { get; set; } = TimeSpan.FromMilliseconds(100);
        public Func<AuthenticateContext, Task<AuthenticateResponse>> AuthenticateAsync { get; set; }
        public Func<HttpRequestContext, Task<HttpRequestMessage>> BeforeHttpRequestSendAsync { get; set; }
        public Func<HttpResponseContext, Task<HttpResponseMessage>> AfterHttpResponseRecivedAsync { get; set; }
    }
}
