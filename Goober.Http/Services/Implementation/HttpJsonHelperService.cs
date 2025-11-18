using Goober.Base.Extensions;
using Goober.Http.Abstractions;
using Goober.Http.Models;
using Goober.Http.Models.Internal;
using Goober.Http.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Goober.Http.Services.Implementation
{
    internal class HttpJsonHelperService : IHttpJsonHelperService
    {
        private static readonly HashSet<int> _acceptableStatusCodes = new HashSet<int> {
            (int)HttpStatusCode.OK,
            (int)HttpStatusCode.Accepted,
            (int)HttpStatusCode.Created,
            207 // HttpStatusCode.MultiStatus
        };
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(
                    namingStrategy: new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: false, processExtensionDataNames: true),
                    allowIntegerValues: true)
            },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCaseExceptDictionaryKeysResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime
        };

        private const string ApplicationJsonContentTypeValue = "application/json";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<AuthenticationEndPointModel> _authenticateParameters;
        private Dictionary<string, Dictionary<string, AuthenticateEndPointDictModel>> _authenticationEndPointsDict;

        public HttpJsonHelperService(
            IHttpClientFactory httpClientFactory,
            IServiceProvider serviceProvider,
            IList<AuthenticationEndPointModel> authenticateEndPointOptions
            )
        {
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
            _authenticateParameters = authenticateEndPointOptions ?? new List<AuthenticationEndPointModel>();
            _authenticationEndPointsDict = GenerateAuthenticationEndPointsDictSafety(_authenticateParameters);
        }

        #region Delete
        public async Task<string> ExecuteDeleteReturnStringAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Delete;

            var requestContext = GetHttpRequestNoContentContextModel(
                    httpMethod: httpMethod,
                    url: urlWithoutQueryParameters,
                    queryParameters: queryParameters,
                    authenticationHeaderValue: authenticationHeaderValue,
                    headerValues: headerValues,
                    jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnStringInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            return strRet;
        }

        public async Task<TResponse> ExecuteDeleteAsync<TResponse>(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Delete;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: httpMethod,
                url: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnStringInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            var ret = Deserialize<TResponse, HttpRequestNoContentModel>(value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);

            return ret;
        }

        public async Task<TResponse> ExecuteDeleteAsync<TResponse, TRequest>(string path,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Delete;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: httpMethod,
                url: path,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var responseContext = await ExecuteRequestReturnResponseContextInternalAsync<TRequest, TResponse>(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            if (responseContext == null)
                return default;

            return responseContext.Response;
        }


        #endregion

        #region Get
        public async Task<string> ExecuteGetReturnStringAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Get;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: httpMethod,
                url: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnStringInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            return strRet;
        }

        public async Task<Stream> ExecuteGetReturnStreamAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Get;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: httpMethod,
                url: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnStreamInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            return strRet;
        }

        public async Task<IFormFile> ExecuteGetReturnFormFileAsync(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Get;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: httpMethod,
                url: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnFormFileInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            return strRet;
        }

        public async Task<TResponse> ExecuteGetAsync<TResponse>(string urlWithoutQueryParameters,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var httpMethod = HttpMethod.Get;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: httpMethod,
                url: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecuteReturnStringInternalAsync(requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                httpMethod: httpMethod);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            var ret = Deserialize<TResponse, HttpRequestNoContentModel>(value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);

            return ret;
        }
        #endregion

        #region Post
        public async Task<TResponse> ExecutePostFormDataAsync<TResponse>(string url,
            List<KeyValuePair<string, string>> formData,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings,
                formData: formData
            );

            var strRet = await ExecutePostReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return Deserialize<TResponse, HttpRequestNoContentModel>(
                value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);
        }

        public async Task<TResponse> ExecutePostAsync<TResponse, TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings
            );

            var strRet = await ExecutePostReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return Deserialize<TResponse, TRequest>(
                value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);
        }

        public async Task<string> ExecutePostFormDataReturnStringAsync<TResponse>(string url,
            List<KeyValuePair<string, string>> formData,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var requestContext = GetHttpRequestNoContentContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings,
                formData: formData);

            var strRet = await ExecutePostReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return strRet;
        }

        public async Task<string> ExecutePostReturnStringAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings);

            var ret = await ExecutePostReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            return ret;
        }

        public async Task<Stream> ExecutePostReturnStreamAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings);

            var ret = await ExecutePostReturnStreamInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            return ret;
        }

        public async Task<IFormFile> ExecutePostReturnFormFileAsync<TRequest>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings);

            var ret = await ExecutePostReturnFormFileInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            return ret;
        }

        public async Task<HttpResponseContextModel<TResponse>> ExecutePostReturnResponseContextAsync<TRequest, TResponse>(string url,
            TRequest request,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = GetHttpRequestContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings);

            var ret = await ExecutePostReturnResponseContextInternalAsync<TRequest, TResponse>(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            return ret;
        }
        #endregion

        #region Put
        public async Task<TResponse> ExecutePutAsync<TResponse, TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {

            var requestContext = GetHttpRequestContextModel(httpMethod: HttpMethod.Put,
                url: url,
                request: request,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecutePutReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return Deserialize<TResponse, TRequest>(
                value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);

        }

        public async Task<string> ExecutePutReturnStringAsync<TRequest>(string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var requestContext = GetHttpRequestContextModel(httpMethod: HttpMethod.Put,
                url: url,
                request: request,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecutePutReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return strRet;
        }


        #endregion

        #region Upload
        public async Task<TResponse> UploadFileAsync<TResponse>(string url,
            IFormFile file,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var strRet = await UploadFileReturnStringAsync(url: url,
                file: file,
                formDataFileParameterName: formDataFileParameterName,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings,
                timeoutInMilliseconds: timeoutInMilliseconds,
                responseMaxContentLength: responseMaxContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;
            var listOfFiles = new List<string> { $"{formDataFileParameterName}:{file.FileName};contentType:{file.ContentType};fileLength:{file.Length}" };

            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(url);

            var loggingRequestContext = GetHttpRequestNoContentContextModel(
                httpMethod: HttpMethod.Post,
                url: url,
                authenticationHeaderValue: authenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, url),
                headerValues: headerValues,
                jsonSerializerSettings: serializerSettings,
                files: listOfFiles);

            return Deserialize<TResponse, HttpRequestNoContentModel>(
                value: strRet,
                jsonSerializerSettings: loggingRequestContext.JsonSerializerSettings,
                loggingRequestContext: loggingRequestContext);
        }

        public async Task<string> UploadFileReturnStringAsync(string url,
            IFormFile file,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(url);

            return await ExecuteWithPolicyIfExistsAsync(url, async () =>
            {
                using (var fileStream = file.OpenReadStream())
                {
                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                        var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                            requestUrl: url,
                            httpMethodType: HttpMethod.Post,
                            authenticationHeaderValue: authenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, url),
                            headerValues: null,
                            responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                        var fileStreamContent = new StreamContent(fileStream);
                        var fileHeaders = file.Headers.ToList();

                        foreach (var headerItem in fileHeaders)
                        {
                            fileStreamContent.Headers.Add(headerItem.Key, headerItem.Value.ToArray());
                        }

                        var formData = new MultipartFormDataContent
                        {
                            {
                                fileStreamContent, formDataFileParameterName, file.FileName
                            }
                        };

                        httpRequest.Content = formData;

                        foreach (var iCustomHeader in headerValues)
                        {
                            httpRequest.Content.Headers.Add(iCustomHeader.Key, iCustomHeader.Value);
                        }

                        var httpResponse = await httpClient.SendAsync(httpRequest);

                        if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                        {
                            return default;
                        }

                        var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;
                        var listOfFiles = new List<string>
                        {
                            $"{formDataFileParameterName}:{file.FileName};contentType:{file.ContentType};fileLength:{file.Length}"
                        };


                        var loggingRequestContext = GetHttpRequestNoContentContextModel(
                            httpMethod: HttpMethod.Post,
                            url: url,
                            authenticationHeaderValue: authenticationHeaderValue,
                            headerValues: headerValues,
                            jsonSerializerSettings: serializerSettings,
                            files: listOfFiles);

                        var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                            httpResponse: httpResponse,
                            loggingRequestContext: loggingRequestContext,
                            maxResponseContentLength: responseMaxContentLength);

                        return ret;
                    }
                }
            });
        }

        #endregion

        #region PATCH

        public async Task<TResponse> ExecutePatchAsync<TResponse, TRequest>(
            string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var requestContext = GetHttpRequestContextModel(httpMethod: new HttpMethod(HttpGlossary.PATCH),
                url: url,
                request: request,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecutePatchReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            if (string.IsNullOrEmpty(strRet) == true)
                return default;

            return Deserialize<TResponse, TRequest>(
                value: strRet,
                jsonSerializerSettings: requestContext.JsonSerializerSettings,
                loggingRequestContext: requestContext);
        }

        public async Task<string> ExecutePatchReturnStringAsync<TRequest>(
            string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var requestContext = GetHttpRequestContextModel(httpMethod: new HttpMethod(HttpGlossary.PATCH),
                url: url,
                request: request,
                queryParameters: queryParameters,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues,
                jsonSerializerSettings: jsonSerializerSettings);

            var strRet = await ExecutePatchReturnStringInternalAsync(
                requestContext: requestContext,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength);

            return strRet;
        }

        #endregion

        #region execute internal

        private async Task<string> ExecuteReturnStringInternalAsync(
            HttpRequestContextModel<HttpRequestNoContentModel> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength,
            HttpMethod httpMethod)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var url = HttpUtils.BuildUrlWithQueryParameters(requestContext.Url, requestContext.QueryParameters);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: url,
                        httpMethodType: httpMethod,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues,
                        responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest);
                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<Stream> ExecuteReturnStreamInternalAsync(
            HttpRequestContextModel<HttpRequestNoContentModel> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength,
            HttpMethod httpMethod)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var url = HttpUtils.BuildUrlWithQueryParameters(requestContext.Url, requestContext.QueryParameters);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: url,
                        httpMethodType: httpMethod,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return new MemoryStream();
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseStreamAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<IFormFile> ExecuteReturnFormFileInternalAsync(
            HttpRequestContextModel<HttpRequestNoContentModel> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength,
            HttpMethod httpMethod)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var url = HttpUtils.BuildUrlWithQueryParameters(requestContext.Url, requestContext.QueryParameters);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: url,
                        httpMethodType: httpMethod,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseFormFileAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<string> ExecutePostReturnStringInternalAsync<TRequest>(
           HttpRequestContextModel<TRequest> requestContext,
           int timeoutInMilliseconds,
           long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: HttpMethod.Post,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues,
                        responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                    httpRequest.Content = GetPostContent(requestContext);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }
        private async Task<Stream> ExecutePostReturnStreamInternalAsync<TRequest>(
           HttpRequestContextModel<TRequest> requestContext,
           int timeoutInMilliseconds,
           long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: HttpMethod.Post,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest.Content = GetPostContent(requestContext);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return new MemoryStream();
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseStreamAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<IFormFile> ExecutePostReturnFormFileInternalAsync<TRequest>(
           HttpRequestContextModel<TRequest> requestContext,
           int timeoutInMilliseconds,
           long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: HttpMethod.Post,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest.Content = GetPostContent(requestContext);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseFormFileAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<HttpResponseContextModel<TResponse>> ExecutePostReturnResponseContextInternalAsync<TRequest, TResponse>(
           HttpRequestContextModel<TRequest> requestContext,
           int timeoutInMilliseconds,
           long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: HttpMethod.Post,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest.Content = GetPostContent(requestContext);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseContextAndProcessResponseStatusCodeAsync<TRequest, TResponse>(
                        httpResponse: httpResponse,
                        requestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<string> ExecutePutReturnStringInternalAsync<TRequest>(
            HttpRequestContextModel<TRequest> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: HttpMethod.Put,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues,
                        responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    httpRequest.Content = GetPutContent(requestContext);

                    var httpResponse = await httpClient.SendAsync(httpRequest);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<string> ExecutePatchReturnStringInternalAsync<TRequest>(
            HttpRequestContextModel<TRequest> requestContext,
            int timeoutInMilliseconds,
            long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequestMessage = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: requestContext.HttpMethod,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues,
                        responseMediaTypes: new List<string> { ApplicationJsonContentTypeValue });

                    httpRequestMessage.Content = GetPatchContent(requestContext);

                    var httpResponse = await httpClient.SendAsync(httpRequestMessage);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    var ret = await GetResponseStringAndProcessResponseStatusCodeAsync(
                        httpResponse: httpResponse,
                        loggingRequestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }

        private async Task<T> ExecuteWithPolicyIfExistsAsync<T>(string requestUrl, Func<Task<T>> func)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestUrl);

            var endpoint = GetEndpoint(endPointSchemeAndHost, requestUrl);

            if (endpoint != null && endpoint.Count > 0)
            {
                return await endpoint.Values.First().RetryPolicy.ExecuteAsync(async () => await func());
            }

            return await func();
        }

        private async Task<HttpRequestMessage> ExecuteHookBeforeHttpRequestSendAsync(string endPointSchemeAndHost, HttpRequestMessage httpRequestMessage)
        {
            var norm = NormalizeSchemeAndHost(endPointSchemeAndHost);
            if (_authenticationEndPointsDict.TryGetValue(norm, out var aliasDict) == true)
            {
                if (aliasDict.Count == 0)
                {
                    return httpRequestMessage;
                }

                if (aliasDict.Values.FirstOrDefault()?.EndPointOptions?.BeforeHttpRequestSendAsync != null)
                    httpRequestMessage = await aliasDict.Values.First()
                        .EndPointOptions
                        .BeforeHttpRequestSendAsync?
                        .Invoke(new HttpRequestContext { HttpRequestMessage = httpRequestMessage, ServiceProvider = _serviceProvider });

                return httpRequestMessage;
            }

            return httpRequestMessage;
        }

        private async Task ExecuteHookAfterHttpResponseRecivedAsync(string endPointSchemeAndHost, HttpResponseMessage httpResponseMessage)
        {
            var norm = NormalizeSchemeAndHost(endPointSchemeAndHost);
            if (_authenticationEndPointsDict.TryGetValue(norm, out var aliasDict) == true)
            {
                if (aliasDict.Count == 0)
                {
                    return;
                }

                if (aliasDict.Values.FirstOrDefault()?.EndPointOptions?.AfterHttpResponseRecivedAsync != null)
                    await aliasDict.Values.First()
                        .EndPointOptions
                        .AfterHttpResponseRecivedAsync?
                        .Invoke(new HttpResponseContext { HttpResponseMessage = httpResponseMessage, ServiceProvider = _serviceProvider });

                return;
            }

            return;
        }

        private async Task<HttpResponseContextModel<TResponse>> ExecuteRequestReturnResponseContextInternalAsync<TRequest, TResponse>(
           HttpRequestContextModel<TRequest> requestContext,
           HttpMethod httpMethod,
           int timeoutInMilliseconds,
           long maxResponseContentLength)
        {
            var endPointSchemeAndHost = HttpUtils.GetSchemeAndHost(requestContext.Url);

            return await ExecuteWithPolicyIfExistsAsync(requestContext.Url, async () =>
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.Timeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);

                    var httpRequest = HttpUtils.GenerateHttpRequestMessage(
                        requestUrl: requestContext.Url,
                        httpMethodType: httpMethod,
                        authenticationHeaderValue: requestContext.AuthenticationHeaderValue ?? await GetAuthHeaderAsync(endPointSchemeAndHost, requestContext.Url),
                        headerValues: requestContext.HeaderValues);

                    httpRequest.Content = GetPostContent(requestContext);

                    httpRequest = await ExecuteHookBeforeHttpRequestSendAsync(endPointSchemeAndHost, httpRequest);

                    var httpResponse = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

                    if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    await ExecuteHookAfterHttpResponseRecivedAsync(endPointSchemeAndHost, httpResponse);

                    var ret = await GetResponseContextAndProcessResponseStatusCodeAsync<TRequest, TResponse>(
                        httpResponse: httpResponse,
                        requestContext: requestContext,
                        maxResponseContentLength: maxResponseContentLength);

                    return ret;
                }
            });
        }



        #endregion

        #region private methods

        private static HttpContent GetPostContent<TRequest>(HttpRequestContextModel<TRequest> requestContext)
        {
            if (requestContext.RequestContent != null)
            {
                var strJsonContent = Serialize(requestContext.RequestContent, requestContext.JsonSerializerSettings);

                return new StringContent(content: strJsonContent, Encoding.UTF8, mediaType: ApplicationJsonContentTypeValue);
            }

            if (requestContext.FormContent != null && requestContext.FormContent.Any() == true)
                return new FormUrlEncodedContent(requestContext.FormContent);

            throw new NotSupportedException($"Can't get post http content, request: {Serialize(requestContext, requestContext.JsonSerializerSettings)} ");
        }

        private static HttpContent GetPutContent<TRequest>(HttpRequestContextModel<TRequest> requestContext)
        {
            if (requestContext.RequestContent == null)
                throw new NotSupportedException($"Can't get put http content, request: {Serialize(requestContext, requestContext.JsonSerializerSettings)} ");

            var strJsonContent = Serialize(requestContext.RequestContent, requestContext.JsonSerializerSettings);
            return new StringContent(content: strJsonContent, Encoding.UTF8, mediaType: ApplicationJsonContentTypeValue);
        }

        private static HttpContent GetPatchContent<TRequest>(HttpRequestContextModel<TRequest> requestContext)
        {
            if (requestContext.RequestContent == null)
                throw new NotSupportedException($"Can't get patch http content, request: {Serialize(requestContext, requestContext.JsonSerializerSettings)} ");

            var strJsonContent = Serialize(requestContext.RequestContent, requestContext.JsonSerializerSettings);
            return new StringContent(content: strJsonContent, Encoding.UTF8, mediaType: ApplicationJsonContentTypeValue);
        }

        private HttpRequestContextModel<HttpRequestNoContentModel> GetHttpRequestNoContentContextModel(
            HttpMethod httpMethod,
            string url,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            List<KeyValuePair<string, string>> formData = null,
            List<string> files = null)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = new HttpRequestContextModel<HttpRequestNoContentModel>
            {
                HttpMethod = httpMethod,
                Url = url,
                QueryParameters = queryParameters,
                AuthenticationHeaderValue = authenticationHeaderValue,
                HeaderValues = headerValues,
                JsonSerializerSettings = serializerSettings,
                FormContent = formData,
                Files = files
            };

            return requestContext;
        }

        private HttpRequestContextModel<TRequest> GetHttpRequestContextModel<TRequest>(
            HttpMethod httpMethod,
            string url,
            TRequest request,
            List<KeyValuePair<string, string>> queryParameters = null,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            var serializerSettings = jsonSerializerSettings ?? _jsonSerializerSettings;

            var requestContext = new HttpRequestContextModel<TRequest>
            {
                HttpMethod = httpMethod,
                Url = url,
                RequestContent = request,
                QueryParameters = queryParameters,
                AuthenticationHeaderValue = authenticationHeaderValue,
                HeaderValues = headerValues,
                JsonSerializerSettings = serializerSettings
            };

            return requestContext;
        }

        private static string Serialize(object value, JsonSerializerSettings serializerSettings = null)
        {
            return JsonConvert.SerializeObject(value, serializerSettings ?? _jsonSerializerSettings);
        }

        private static TTarget Deserialize<TTarget, TRequest>(string value,
            JsonSerializerSettings jsonSerializerSettings,
            HttpRequestContextModel<TRequest> loggingRequestContext)
        {
            try
            {
                return JsonConvert.DeserializeObject<TTarget>(value, jsonSerializerSettings ?? _jsonSerializerSettings);
            }
            catch (Exception exc)
            {
                throw new WebException(
                    message: $"Can't deserialize to type = \"{typeof(TTarget).Name}\", message = \"{exc.Message}\" from value = \"{value}\", request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)}",
                    innerException: exc);
            }
        }

        private static async Task<string> GetResponseStringAndProcessResponseStatusCodeAsync<TRequest>(HttpResponseMessage httpResponse,
            HttpRequestContextModel<TRequest> loggingRequestContext,
            long maxResponseContentLength)
        {
            var (isReadToTheEnd, stringResult) = await ReadContentWithMaxSizeRetrictionAsync(httpResponse.Content,
                encoding: Encoding.UTF8,
                maxSize: maxResponseContentLength);

            if (_acceptableStatusCodes.Contains((int)httpResponse.StatusCode))
                return isReadToTheEnd
                    ? stringResult.ToString()
                    : throw new WebException($"Response content length is greater than {maxResponseContentLength}, request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)} ");

            var generalErrorMessage =
                $"Request fault with statusCode = {httpResponse.StatusCode}, response: {stringResult}, request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)}";

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException(generalErrorMessage);
            }

            var exception = new WebException(generalErrorMessage);
            exception.Data.Add("StatusCode", httpResponse.StatusCode);

            if (isReadToTheEnd == false)
            {
                stringResult.AppendLine();
                stringResult.Append($"<<< NOT END, response size is greater than {maxResponseContentLength.ToString()}");
            }

            throw exception;
        }

        private static async Task<Stream> GetResponseStreamAndProcessResponseStatusCodeAsync<TRequest>(HttpResponseMessage httpResponse,
            HttpRequestContextModel<TRequest> loggingRequestContext,
            long maxResponseContentLength)
        {
            if (_acceptableStatusCodes.Contains((int)httpResponse.StatusCode))
                return await httpResponse.Content.ReadAsStreamAsync();

            var (isReadToTheEnd, stringResult) = await ReadContentWithMaxSizeRetrictionAsync(httpResponse.Content,
                encoding: Encoding.UTF8,
                maxSize: maxResponseContentLength);

            var generalErrorMessage =
                $"Request fault with statusCode = {httpResponse.StatusCode}, response: {stringResult}, request: {Serialize(loggingRequestContext, loggingRequestContext.JsonSerializerSettings)}";

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException(generalErrorMessage);
            }

            var exception = new WebException(generalErrorMessage);

            if (isReadToTheEnd == false)
            {
                stringResult.AppendLine();
                stringResult.Append($"<<< NOT END, response size is greater than {maxResponseContentLength.ToString()}");
            }

            throw exception;
        }
        private static async Task<IFormFile> GetResponseFormFileAndProcessResponseStatusCodeAsync<TRequest>(HttpResponseMessage httpResponse,
            HttpRequestContextModel<TRequest> loggingRequestContext,
            long maxResponseContentLength)
        {
            var baseStream = await GetResponseStreamAndProcessResponseStatusCodeAsync(
                httpResponse: httpResponse,
                loggingRequestContext: loggingRequestContext,
                maxResponseContentLength: maxResponseContentLength);

            var contentDispositionHeaderValue = httpResponse.Content.Headers.ContentDisposition;

            var length = contentDispositionHeaderValue?.Size ?? httpResponse.Content.Headers.ContentLength ?? 0;

            return new FormFile(
                baseStream: baseStream,
                baseStreamOffset: 0,
                length: length,
                name: contentDispositionHeaderValue?.Name?.ToString() ?? string.Empty,
                fileName: contentDispositionHeaderValue?.FileName?.ToString() ?? string.Empty,
                headers: httpResponse.Content.Headers);
        }

        private static async Task<HttpResponseContextModel<TResponse>> GetResponseContextAndProcessResponseStatusCodeAsync<TRequest, TResponse>(
            HttpResponseMessage httpResponse,
            HttpRequestContextModel<TRequest> requestContext,
            long maxResponseContentLength)
        {
            var responseString = await GetResponseStringAndProcessResponseStatusCodeAsync(
                httpResponse: httpResponse,
                loggingRequestContext: requestContext,
                maxResponseContentLength: maxResponseContentLength);

            return new HttpResponseContextModel<TResponse>()
            {
                Response = Deserialize<TResponse, TRequest>(responseString,
                            jsonSerializerSettings: requestContext.JsonSerializerSettings,
                            loggingRequestContext: requestContext),
                HeaderValues = httpResponse.Headers?.Select(x => new KeyValuePair<string, string>(x.Key, string.Join(";", x.Value))).ToList(),
            };
        }

        private static async Task<(bool IsReadToTheEnd, StringBuilder StringResult)> ReadContentWithMaxSizeRetrictionAsync(HttpContent httpContent,
                    Encoding encoding,
                    long maxSize,
                    int bufferSize = 1024)
        {
            if ((httpContent.Headers.ContentLength.HasValue) &&
                (httpContent.Headers.ContentLength.Value < maxSize))
            {
                byte[] contentArray = await httpContent.ReadAsByteArrayAsync();
                var sbResult = new StringBuilder(encoding.GetString(bytes: contentArray, index: 0, count: contentArray.Length));
                return (true, sbResult);
            }

            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                if (stream.CanRead == false)
                {
                    throw new InvalidOperationException("Stream is not ready to ready");
                }

                var totalBytesRead = 0;

                byte[] contentArray = new byte[maxSize + bufferSize];
                byte[] buffer = new byte[bufferSize];

                var bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);

                while ((bytesRead > 0) && (totalBytesRead <= maxSize))
                {
                    Array.Copy(buffer, 0, contentArray, totalBytesRead, bytesRead);
                    totalBytesRead += bytesRead;
                    bytesRead = await stream.ReadAsync(buffer, 0, bufferSize);
                }

                var sbResult = new StringBuilder(encoding.GetString(bytes: contentArray, index: 0, count: totalBytesRead));
                return (totalBytesRead <= maxSize, sbResult);
            }
        }

        private Dictionary<string, Dictionary<string, AuthenticateEndPointDictModel>> GenerateAuthenticationEndPointsDictSafety(IList<AuthenticationEndPointModel> authenticationEndPoints)
        {
            var ret = new Dictionary<string, Dictionary<string, AuthenticateEndPointDictModel>>();

            if (authenticationEndPoints == null)
            {
                return ret;
            }

            foreach (var iEndPoint in authenticationEndPoints)
            {
                var normalizedSchemeAndHost = NormalizeSchemeAndHost(iEndPoint.EndPointSchemeAndHost);

                var normalizedAlias = iEndPoint.EndPointAlias.ToLowerAndTrimSafety();

                if (string.IsNullOrEmpty(normalizedSchemeAndHost))
                {
                    continue;
                }

                ret.TryGetValue(normalizedSchemeAndHost, out var aliasDict);

                if (aliasDict == null)
                {
                    aliasDict = new Dictionary<string, AuthenticateEndPointDictModel>();
                    ret.Add(normalizedSchemeAndHost, aliasDict);
                }

                if (aliasDict.ContainsKey(normalizedAlias) == true)
                {
                    continue;
                }

                var endPoint = new AuthenticateEndPointDictModel
                {
                    EndPointOptions = new AuthenticationEndPointModel
                    {
                        EndPointAlias = normalizedAlias,
                        EndPointSchemeAndHost = normalizedSchemeAndHost,
                        AuthenticateAsync = iEndPoint.AuthenticateAsync,
                        BeforeHttpRequestSendAsync = iEndPoint.BeforeHttpRequestSendAsync,
                        AfterHttpResponseRecivedAsync = iEndPoint.AfterHttpResponseRecivedAsync,
                        RetryCount = iEndPoint.RetryCount,
                        RetryPeriod = iEndPoint.RetryPeriod
                    },
                    RetryPolicy = GenerateUnauthorizedRetryPolicy(iEndPoint)
                };

                aliasDict.Add(key: normalizedSchemeAndHost, value: endPoint);
            }

            return ret;
        }

        private AsyncRetryPolicy GenerateUnauthorizedRetryPolicy(AuthenticationEndPointModel authenticateEndPointOptions)
        {
            return Policy.Handle<AuthenticationException>(exc =>
            {
                var authenticateContext = new AuthenticateContext
                {
                    ServiceProvider = _serviceProvider,
                    EndPointSchemeAndHost = authenticateEndPointOptions.EndPointSchemeAndHost,
                    EndPointAlias = authenticateEndPointOptions.EndPointAlias
                };

                var authResult = AsyncHelper.RunSync(() => GetAuthHeaderAsync(authenticateEndPointOptions.EndPointSchemeAndHost));

                if (authResult == null)
                    return false;

                return true;
            })
                .WaitAndRetryAsync(
                    retryCount: authenticateEndPointOptions.RetryCount,
                    sleepDurationProvider: retryAttempt => authenticateEndPointOptions.RetryPeriod);
        }

        public async Task<AuthenticationHeaderValue> GetAuthHeaderAsync(string schemeAndHost, string requestUrl = null)
        {
            var endpoint = GetEndpoint(schemeAndHost, requestUrl);

            var authEndPointOption = endpoint?.Values.FirstOrDefault();

            if (authEndPointOption?.EndPointOptions == null
                || string.IsNullOrEmpty(authEndPointOption?.EndPointOptions?.EndPointSchemeAndHost) == true
                || string.IsNullOrEmpty(authEndPointOption?.EndPointOptions?.EndPointAlias) == true
                || authEndPointOption?.EndPointOptions?.AuthenticateAsync == null)
            {
                return null;
            }

            var authenticateResponse = await authEndPointOption.EndPointOptions?.AuthenticateAsync(
                new AuthenticateContext
                {
                    ServiceProvider = _serviceProvider,
                    EndPointSchemeAndHost = authEndPointOption.EndPointOptions.EndPointSchemeAndHost,
                    EndPointAlias = authEndPointOption.EndPointOptions.EndPointAlias,
                    RequestUrl = requestUrl
                });

            if (string.IsNullOrEmpty(authenticateResponse?.Token) == true)
            {
                return null;
            }

            var tokenType = authenticateResponse.TokenType;
            if (string.IsNullOrEmpty(tokenType) == true)
            {
                tokenType = "Bearer";
            }

            return new AuthenticationHeaderValue(tokenType, authenticateResponse.Token);
        }

        private static string NormalizeSchemeAndHost(string schemeAndHost)
        {
            return schemeAndHost.ToLowerAndTrimSafety()?.Trim("/".ToCharArray());
        }

        private Dictionary<string, AuthenticateEndPointDictModel> GetEndpoint(string schemeAndHost, string requestUrl = null)
        {
            var normalizedSchemeAndHost = NormalizeSchemeAndHost(schemeAndHost);

            _authenticationEndPointsDict.TryGetValue(normalizedSchemeAndHost, out var endpoint);

            if ((endpoint != null && endpoint.Count > 0) || string.IsNullOrEmpty(requestUrl))
            {
                return endpoint;
            }

            // Если не найден endpoint по полному совпадению SchemeAndHost, то пытаемся найти endpoint
            // по совпадению с началом URL запроса. Применимо для случая, если в конфигурации endpoint-а в
            // поле SchemeAndHost указан адрес с базовым путем.
            var endpointWithBasePathKey = _authenticationEndPointsDict.Keys
                .Where(e => requestUrl.StartsWith(e))
                .OrderByDescending(e => e.Length)
                .FirstOrDefault();

            if (endpointWithBasePathKey == null)
            {
                return null;
            }

            _authenticationEndPointsDict.TryGetValue(endpointWithBasePathKey, out var endpointWithBasePath);
            return endpointWithBasePath;
        }
        #endregion
    }
}
