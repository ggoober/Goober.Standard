using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goober.Base.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<(bool IsReadToTheEnd, StringBuilder StringResult)> ReadStreamWithMaxSizeRetrictionAsync(
            this Stream stream,
            Encoding encoding,
            long maxSize,
            int bufferSize = 1024)
        {
            if (stream.CanRead == false)
            {
                throw new InvalidOperationException("stream is not ready to ready");
            }

            var totalBytesRead = 0;

            var sbResult = new StringBuilder();

            byte[] buffer = new byte[bufferSize];
            var bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);

            while (bytesRead > 0)
            {
                var prevTotalBytesRead = totalBytesRead;
                totalBytesRead += bytesRead;

                //Приведение допустимо, так как значение разницы не превышает значения bytesRead с типом int
                var expectedDataLength = totalBytesRead <= maxSize ? bytesRead : (int)(maxSize - prevTotalBytesRead);
                sbResult.Append(encoding.GetString(bytes: buffer, index: 0, count: expectedDataLength));
                if (totalBytesRead > maxSize)
                {
                    return (false, sbResult);
                }
                //Добавляется единица, чтобы можно было отличить, достигнут конец потока или сработало ограничение
                bufferSize = Math.Min(bufferSize, (int)(maxSize - totalBytesRead + 1));
                bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
            }

            return (true, sbResult);
        }

        public static async Task<(bool IsReadToTheEnd, byte[] Bytes)> ReadStreamBytesWithMaxSizeRetrictionAsync(
            this Stream stream,
            long maxSize,
            int bufferSize = 1024)
        {
            if (stream.CanRead == false)
            {
                throw new InvalidOperationException("stream is not ready to ready");
            }

            var result = new List<byte>();
            var buffer = new byte[bufferSize];

            var bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
            while (bytesRead > 0)
            {
                var totalBytesRead = result.Count + bytesRead;
                //Приведение допустимо, так как значение разницы не превышает значения bytesRead с типом int
                var expectedDataLength = totalBytesRead <= maxSize ? bytesRead : (int)(maxSize - result.Count);
                result.AddRange(buffer.Take(expectedDataLength));
                if (totalBytesRead > maxSize)
                {
                    return (false, result.ToArray());
                }
                //Добавляется единица, чтобы можно было отличить, достигнут конец потока или сработало ограничение
                bufferSize = Math.Min(bufferSize, (int)(maxSize - totalBytesRead + 1));
                bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
            }

            return (true, result.ToArray());
        }
    }
}
