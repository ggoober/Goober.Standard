using System.Text;
using Goober.Base.Extensions;

namespace Goober.Standard.Tests.Base
{
    public class StreamExtensionsTests
    {
        [Theory]
        [InlineData(90, 100, 70)]
        [InlineData(90, 100, 90)]
        [InlineData(90, 100, 100)]
        [InlineData(90, 100, 110)]
        [InlineData(90, 200, 70)]
        [InlineData(90, 200, 90)]
        [InlineData(90, 200, 100)]
        [InlineData(90, 200, 110)]
        public async Task ReadStreamBytesWithMaxSizeRetrictionAsync_SourceLengthLessThanMaxSize_ReadFullSource(
            int sourceDataLength,
            int readingDataLength,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamBytesWithMaxSizeRetrictionAsync(readingDataLength, bufferSize: bufferSize);

            Assert.True(readedData.IsReadToTheEnd);
            Assert.NotNull(readedData.Bytes);
            Assert.Equal(sourceData.Length, readedData.Bytes.Length);
            Assert.Equal<byte>(sourceData, readedData.Bytes);
        }

        [Theory]
        [InlineData(100, 100, 70)]
        [InlineData(100, 100, 100)]
        [InlineData(100, 100, 110)]
        public async Task ReadStreamBytesWithMaxSizeRetrictionAsync_SourceLengthEqualsMaxSize_ReadFullSource(
            int sourceDataLength,
            int readingDataLength,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamBytesWithMaxSizeRetrictionAsync(readingDataLength, bufferSize: bufferSize);

            Assert.True(readedData.IsReadToTheEnd);
            Assert.NotNull(readedData.Bytes);
            Assert.Equal(sourceData.Length, readedData.Bytes.Length);
            Assert.Equal<byte>(sourceData, readedData.Bytes);
        }

        [Theory]
        [InlineData(100, 90, 70)]
        [InlineData(100, 90, 90)]
        [InlineData(100, 90, 100)]
        [InlineData(100, 90, 110)]
        [InlineData(200, 90, 70)]
        [InlineData(200, 90, 90)]
        [InlineData(200, 90, 100)]
        [InlineData(200, 90, 110)]
        public async Task ReadStreamBytesWithMaxSizeRetrictionAsync_SourceLengthGreaterThanMaxSize_ReadMaxSize(
            int sourceDataLength,
            int maxSize,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamBytesWithMaxSizeRetrictionAsync(maxSize, bufferSize: bufferSize);

            var trimmedSourceData = sourceData.Take(maxSize).ToArray();

            Assert.False(readedData.IsReadToTheEnd);
            Assert.NotNull(readedData.Bytes);
            Assert.Equal(trimmedSourceData.Length, readedData.Bytes.Length);
            Assert.Equal<byte>(trimmedSourceData, readedData.Bytes);
        }

        [Theory]
        [InlineData(90, 100, 70)]
        [InlineData(90, 100, 90)]
        [InlineData(90, 100, 100)]
        [InlineData(90, 100, 110)]
        [InlineData(90, 200, 70)]
        [InlineData(90, 200, 90)]
        [InlineData(90, 200, 100)]
        [InlineData(90, 200, 110)]
        public async Task ReadStreamWithMaxSizeRetrictionAsync_SourceLengthLessThanMaxSize_ReadFullSource(
            int sourceDataLength,
            int readingDataLength,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamWithMaxSizeRetrictionAsync(Encoding.UTF8, readingDataLength, bufferSize: bufferSize);
            var sourceString = Encoding.UTF8.GetString(sourceData);
            var readedString = readedData.StringResult.ToString();

            Assert.True(readedData.IsReadToTheEnd);
            Assert.NotNull(readedString);
            Assert.Equal(sourceString.Length, readedString.Length);
            Assert.Equal(sourceString, readedString);
        }

        [Theory]
        [InlineData(100, 100, 70)]
        [InlineData(100, 100, 100)]
        [InlineData(100, 100, 110)]
        public async Task ReadStreamWithMaxSizeRetrictionAsync_SourceLengthEqualsMaxSize_ReadFullSource(
            int sourceDataLength,
            int readingDataLength,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamWithMaxSizeRetrictionAsync(Encoding.UTF8, readingDataLength, bufferSize: bufferSize);
            var sourceString = Encoding.UTF8.GetString(sourceData);
            var readedString = readedData.StringResult.ToString();

            Assert.True(readedData.IsReadToTheEnd);
            Assert.NotNull(readedString);
            Assert.Equal(sourceString.Length, readedData.StringResult.Length);
            Assert.Equal(sourceString, readedData.StringResult.ToString());
        }

        [Theory]
        [InlineData(100, 90, 70)]
        [InlineData(100, 90, 90)]
        [InlineData(100, 90, 100)]
        [InlineData(100, 90, 110)]
        [InlineData(200, 90, 70)]
        [InlineData(200, 90, 90)]
        [InlineData(200, 90, 100)]
        [InlineData(200, 90, 110)]
        public async Task ReadStreamWithMaxSizeRetrictionAsync_SourceLengthGreaterThanMaxSize_ReadMaxSize(
            int sourceDataLength,
            int maxSize,
            int bufferSize)
        {
            var sourceData = new byte[sourceDataLength];
            Random.Shared.NextBytes(sourceData);
            using MemoryStream stream = new MemoryStream(sourceData);

            var readedData = await stream.ReadStreamWithMaxSizeRetrictionAsync(Encoding.UTF8, maxSize, bufferSize: bufferSize);

            var trimmedSourceData = sourceData.Take(maxSize).ToArray();
            var sourceString = Encoding.UTF8.GetString(trimmedSourceData);
            var readedString = readedData.StringResult.ToString();

            Assert.False(readedData.IsReadToTheEnd);
            Assert.NotNull(readedString);
            Assert.Equal(sourceString.Length, readedString.Length);
            Assert.Equal(sourceString, readedString);
        }
    }
}
