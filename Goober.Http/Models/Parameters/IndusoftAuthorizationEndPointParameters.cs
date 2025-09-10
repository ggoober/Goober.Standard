using System;

namespace Goober.Http.Models.Parameters
{
    public class IndusoftAuthorizationEndPointParameters
    {
        public string Alias { get; set; }
        public string SchemeAndHost { get; set; }
        public int RetryCount { get; set; } = 1;
        public TimeSpan RetryPeriod { get; set; } = TimeSpan.FromMilliseconds(100);
    }
}
