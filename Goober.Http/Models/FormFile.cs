using Goober.Http.Models.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Goober.Http.Models
{
    //implementation from https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http/src/FormFile.cs
    public class FormFile : IFormFile
    {
        // Stream.CopyTo method uses 80KB as the default buffer size.
        private const int DefaultBufferSize = 80 * 1024;

        private readonly Stream _baseStream;
        private readonly long _baseStreamOffset;

        /// <summary>
        /// Initializes a new instance of <see cref="FormFile"/>.
        /// </summary>
        /// <param name="baseStream">The <see cref="Stream"/> containing the form file.</param>
        /// <param name="baseStreamOffset">The offset at which the form file begins.</param>
        /// <param name="length">The length of the form file.</param>
        /// <param name="name">The name of the form file from the <c>Content-Disposition</c> header.</param>
        /// <param name="fileName">The file name from the <c>Content-Disposition</c> header.</param>
        public FormFile(Stream baseStream, long baseStreamOffset, long length, string name, string fileName, HttpHeaders headers)
        {
            _baseStream = baseStream;
            _baseStreamOffset = baseStreamOffset;
            Length = length;
            Name = name;
            FileName = fileName;
            var headersDictionary = headers != null ?
                headers.ToDictionary(kvp => kvp.Key, kvp => new StringValues(kvp.Value?.ToArray() ?? Array.Empty<string>())) :
                new Dictionary<string, StringValues>();
            Headers = new HeaderDictionary(headersDictionary);
        }

        /// <summary>
        /// Gets the raw <c>Content-Disposition</c> header of the uploaded file.
        /// </summary>
        public string ContentDisposition
        {
            get { return Headers[HeaderNames.ContentDisposition]; }
        }

        /// <summary>
        /// Gets the raw <c>Content-Type</c> header of the uploaded file.
        /// </summary>
        public string ContentType
        {
            get { return Headers[HeaderNames.ContentType]; }
        }

        /// <summary>
        /// Gets the header dictionary of the uploaded file.
        /// </summary>
        public IHeaderDictionary Headers { get; set; } = default;

        /// <summary>
        /// Gets the file length in bytes.
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Gets the name from the <c>Content-Disposition</c> header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the file name from the <c>Content-Disposition</c> header.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Opens the request stream for reading the uploaded file.
        /// </summary>
        public Stream OpenReadStream()
        {
            return new ReferenceReadStream(_baseStream, _baseStreamOffset, Length);
        }

        /// <summary>
        /// Copies the contents of the uploaded file to the <paramref name="target"/> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        public void CopyTo(Stream target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using (var readStream = OpenReadStream())
            {
                readStream.CopyTo(target, DefaultBufferSize);
            }
        }

        /// <summary>
        /// Asynchronously copies the contents of the uploaded file to the <paramref name="target"/> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        /// <param name="cancellationToken"></param>
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using (var readStream = OpenReadStream())
            {
                await readStream.CopyToAsync(target, DefaultBufferSize, cancellationToken);
            }
        }
    }
}
