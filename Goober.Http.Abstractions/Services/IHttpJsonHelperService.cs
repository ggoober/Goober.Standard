using Goober.Http.Abstractions;
using Goober.Http.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Goober.Http.Services
{
    public interface IHttpJsonHelperService
    {
        Task<TResponse> ExecuteDeleteAsync<TResponse>(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecuteDeleteReturnStringAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> ExecuteDeleteAsync<TResponse, TRequest>(string path,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecuteGetReturnStringAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<Stream> ExecuteGetReturnStreamAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<IFormFile> ExecuteGetReturnFormFileAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> ExecuteGetAsync<TResponse>(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecutePostReturnStringAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<Stream> ExecutePostReturnStreamAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<IFormFile> ExecutePostReturnFormFileAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> ExecutePostAsync<TResponse, TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<HttpResponseContextModel<TResponse>> ExecutePostReturnResponseContextAsync<TRequest, TResponse>(
            string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength
            );

        Task<TResponse> ExecutePutAsync<TResponse, TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecutePutReturnStringAsync<TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecutePostFormDataReturnStringAsync<TResponse>(string url,
            List<KeyValuePair<string, string>> formData,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> ExecutePostFormDataAsync<TResponse>(string url,
            List<KeyValuePair<string, string>> formData,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> UploadFileReturnStringAsync(string url,
            IFormFile file,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> UploadFileAsync<TResponse>(string url,
            IFormFile file,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength);

        Task<TResponse> ExecutePatchAsync<TResponse, TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);

        Task<string> ExecutePatchReturnStringAsync<TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength);
    }
}
