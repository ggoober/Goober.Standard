using System;

namespace Goober.Caching.Models
{
    public class CachedEntryInfo
    {
        public DateTime? NextRefreshDateTime { get; set; }

        public int? RefreshTimeInMilliseconds { get; set; }

        public int? RefreshTimeInMinutes
        {
            get => RefreshTimeInMilliseconds / 60000;
            set => RefreshTimeInMilliseconds = value * 60000;
        }

        public DateTime? LastRefreshDateTime { get; set; }

        public DateTime? ExpirationDateTime { get; set; }

        public int? ExpirationTimeInMilliseconds { get; set; }

        public int? ExpirationTimeInMinutes
        {
            get => ExpirationTimeInMilliseconds / 60000;
            set => ExpirationTimeInMilliseconds = value * 60000;
        }
        
        public DateTime? LastAccessDateTime { get; set; }

        public DateTime RowCreatedDateTime { get; set; }

        public bool UseLock { get; set; }

        public bool IsEmpty { get; set; }
    }
}
