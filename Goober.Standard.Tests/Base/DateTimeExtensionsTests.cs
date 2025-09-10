using Goober.Base.Extensions;

namespace Goober.Standard.Tests.Base
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public async Task ExistTimeToday_NullTemplateShift_ReturnTimeToday()
        {
            var time = DateTime.Today;
            var timeShifted = time.GetTimeShifted(null);
            Assert.Equal(time, timeShifted);
        }

        [Fact]
        public async Task ExistTimeToday_EmptyTemplateShift_ReturnTimeToday()
        {
            var time = DateTime.Today;
            var timeShifted = time.GetTimeShifted(string.Empty);
            Assert.Equal(time, timeShifted);
        }

        [Fact]
        public async Task ExistTimeToday_InvalidTemplateShift_ReturnTimeToday()
        {
            var time = DateTime.Today;
            var timeShifted = time.GetTimeShifted("InvalidTemplateShift");
            Assert.Equal(time, timeShifted);
        }

        [Theory]
        [InlineData("2024-12-19T00:00:00Z", "+1y", "2025-12-19T00:00:00Z")]
        [InlineData("2024-12-01T00:00:00Z", "-1y", "2023-12-01T00:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+1mo", "2025-01-19T00:00:00Z")]
        [InlineData("2024-01-01T00:00:00Z", "-1mo", "2023-12-01T00:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+1w", "2024-12-26T00:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-1w", "2024-12-12T00:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+1d", "2024-12-20T00:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-1d", "2024-12-18T00:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+10h", "2024-12-19T10:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-10h", "2024-12-18T14:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+30m", "2024-12-19T00:30:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-30m", "2024-12-18T23:30:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "+30s", "2024-12-19T00:00:30Z")]
        [InlineData("2024-12-19T00:00:00Z", "-30s", "2024-12-18T23:59:30Z")]

        [InlineData("2024-12-19T00:00:00Z", "+100ms", "2024-12-19T00:00:00.100Z")]
        [InlineData("2024-12-19T00:00:00Z", "-100ms", "2024-12-18T23:59:59.900Z")]

        [InlineData("2024-12-19T00:00:00Z", "-29y+1mo-1w-11d-5h+10m-30s+200ms", "1995-12-31T19:09:30.200Z")]

        [InlineData("2024-12-19T00:00:00Z", "-1mo+1bad-1h", "2024-11-18T23:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-1mo+1d-1bad", "2024-11-20T00:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-1bad+1d+1h", "2024-12-20T01:00:00Z")]

        [InlineData("2024-12-19T00:00:00Z", "-1mo1d-1h", "2024-11-18T23:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "-1mo+1d1h", "2024-11-20T00:00:00Z")]
        [InlineData("2024-12-19T00:00:00Z", "1mo+1d+1h", "2024-12-20T01:00:00Z")]
        public async Task ExistTime_TemplateShift_ReturnTimeShifted(string timeRelative, string templateShift, string timeTarget)
        {
            var relative = DateTime.Parse(timeRelative);
            var target = DateTime.Parse(timeTarget);

            var tmeShifted = relative.GetTimeShifted(templateShift);

            Assert.Equal(target, tmeShifted);
        }
    }
}
