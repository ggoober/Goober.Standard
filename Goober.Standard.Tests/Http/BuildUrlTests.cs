using Goober.Http.Utils;

namespace Goober.Http.Tests.Http
{
    public class BuildUrlTests
    {
        public static IEnumerable<object[]> DataToUtlTest => new[]
        {
            new object[] { "https://cloud.inno.indusoft.ru/api/dsp/", "/api/some-entity/get", "https://cloud.inno.indusoft.ru/api/dsp/api/some-entity/get" },
            new object[] { "https://cloud.inno.indusoft.ru/api/dsp", "/api/some-entity/get", "https://cloud.inno.indusoft.ru/api/dsp/api/some-entity/get" },
            new object[] { "https://cloud.inno.indusoft.ru/api/dsp/", "api/some-entity/get", "https://cloud.inno.indusoft.ru/api/dsp/api/some-entity/get" },
            new object[] { "https://cloud.inno.indusoft.ru/api/dsp", "api/some-entity/get", "https://cloud.inno.indusoft.ru/api/dsp/api/some-entity/get" }
        };

        [Theory]
        [MemberData(nameof(DataToUtlTest))]
        public void HaveExpectedBaseUrlAndMethodUrl_ReturnFull(string expectedSchemeAndHost, string expectedUrlPath, string expectedResult)
        { 
            var result = HttpUtils.BuildUrl(
                    schemeAndHost: expectedSchemeAndHost,
                    urlPath: expectedUrlPath
                );

            Assert.Equal(expectedResult, result);
        }
    }
}
