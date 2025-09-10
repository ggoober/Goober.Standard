using Polly.Retry;

namespace Goober.Http.Models.Internal
{
    internal class AuthenticateEndPointDictModel
    {
        public AuthenticationEndPointModel EndPointOptions { get; set; }

        public AsyncRetryPolicy RetryPolicy { get; set; }
    }
}
