using System;

namespace Goober.Base.Services.Implementation
{
    class DateTimeService : IDateTimeService
    {
        public DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public DateTimeOffset GetDateTimeOffsetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
