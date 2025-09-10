using System;

namespace Goober.Base.Services
{
    public interface IDateTimeService
    {
        DateTime GetDateTimeNow();

        DateTimeOffset GetDateTimeOffsetUtcNow();

	}
}