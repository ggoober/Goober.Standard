using Goober.Http.Abstractions;
using Goober.Http.Models;
using Goober.Http.Services;
using Goober.Http.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Goober.Http.Utils.HeaderUtils;
using System.Net.Http;

namespace Goober.Http
{
    public abstract class BaseHttpService
    {
        #region fields

        protected abstract string ApiSchemeAndHostConfigKey { get; set; }

        private const string CallSequenceKey = "i-callsec";

        private const string CallSequenceIdKey = "i-callsec-id";
        
        protected readonly IHttpJsonHelperService HttpJsonHelperService;
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly IConfiguration Configuration;

        #endregion

        #region ctor

        protected BaseHttpService(
            IConfiguration configuration, 
            IHttpJsonHelperService httpJsonHelperService,
            IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            HttpJsonHelperService = httpJsonHelperService;
            HttpContextAccessor = httpContextAccessor;
        }

        #endregion
        
        #region Delete
        protected async Task<TResponse> ExecuteDeleteAsync<TResponse>(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteDeleteAsync<TResponse>(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues, maxResponseContentLength: maxResponseContentLength);

            return result;
        }

        protected async Task<string> ExecuteDeleteReturnStringAsync(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteDeleteReturnStringAsync(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                maxResponseContentLength: maxResponseContentLength);

            return result;
        }

        protected async Task<TResponse> ExecuteDeleteAsync<TResponse, TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            JsonSerializerSettings jsonSerializerSettings = null,
            string overrieApiSchemeAndHost = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var newPath = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);


            var result = await HttpJsonHelperService.ExecuteDeleteAsync<TResponse, TRequest>(
               path: newPath,
               request: request,
               timeoutInMilliseconds: timeoutInMilliseconds,
               authenticationHeaderValue: authenticationHeaderValue,
               headerValues: newHeaderValues,
               maxResponseContentLength: maxResponseContentLength,
               jsonSerializerSettings: jsonSerializerSettings);

            return result;
        }


        #endregion

        #region Get
        protected async Task<TResponse> ExecuteGetAsync<TResponse>(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteGetAsync<TResponse>(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                maxResponseContentLength:maxResponseContentLength);

            return result;
        }

        protected async Task<string> ExecuteGetReturnStringAsync(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteGetReturnStringAsync(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                maxResponseContentLength:maxResponseContentLength);

            return result;
        }

        protected async Task<Stream> ExecuteGetReturnStreamAsync(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteGetReturnStreamAsync(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                maxResponseContentLength: maxResponseContentLength);

            return result;
        }

        protected async Task<IFormFile> ExecuteGetReturnFormFileAsync(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecuteGetReturnFormFileAsync(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                maxResponseContentLength: maxResponseContentLength);

            return result;
        }
        #endregion

        #region Post
        protected async Task<TResponse> ExecutePostAsync<TResponse, TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostAsync<TResponse, TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        protected async Task<string> ExecutePostReturnStringAsync<TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostReturnStringAsync<TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        protected async Task<Stream> ExecutePostReturnStreamAsync<TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostReturnStreamAsync<TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        protected async Task<IFormFile> ExecutePostReturnFormFileAsync<TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostReturnFormFileAsync<TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        protected async Task<TResponse> ExecutePostFormDataAsync<TResponse>(string path,
            List<KeyValuePair<string, string>> formData,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            string overrieApiSchemeAndHost = null,
			int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
			long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostFormDataAsync<TResponse>(
                url: url,
                formData:formData,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength : maxResponseContentLength
            );

            return result;
        }

        protected async Task<string> ExecutePostFormDataReturnStringAsync<TResponse>(string path,
            List<KeyValuePair<string, string>> formData,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            string overrieApiSchemeAndHost = null,
			int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
			long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostFormDataReturnStringAsync<TResponse>(
                url: url,
                formData: formData,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        protected async Task<HttpResponseContextModel<TResponse>> ExecutePostReturnResponseContextAsync<TRequest, TResponse>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePostReturnResponseContextAsync<TRequest, TResponse>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                jsonSerializerSettings: jsonSerializerSettings
            );

            return result;
        }

        #endregion

        #region Put
        protected async Task<TResponse> ExecutePutAsync<TResponse, TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength,
            JsonSerializerSettings jsonSerializerSettings = null
            )
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePutAsync<TResponse, TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                jsonSerializerSettings: jsonSerializerSettings
            );

            return result;
        }

        protected async Task<string> ExecutePutReturnStringAsync<TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePutReturnStringAsync<TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength
            );

            return result;
        }

        #endregion

        #region Upload

        protected async Task<string> UploadFileReturnStringAsync(string path,
            IFormFile file,
            string callerMemberName,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            string overrieApiSchemeAndHost = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);


            var result = await HttpJsonHelperService.UploadFileReturnStringAsync(url: url,
                file: file,
                formDataFileParameterName: formDataFileParameterName,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                responseMaxContentLength: responseMaxContentLength
            );

            return result;
        }

        protected async Task<TResponse> UploadFileAsync<TResponse>(string path,
            IFormFile file,
            string callerMemberName,
            string formDataFileParameterName = "file",
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            string overrieApiSchemeAndHost = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            long responseMaxContentLength = HttpGlossary.MaxResponseContentLength)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.UploadFileAsync<TResponse>(url: url,
                file: file,
                formDataFileParameterName: formDataFileParameterName,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                responseMaxContentLength: responseMaxContentLength
            );

            return result;
        }

        #endregion

        #region PATCH
        protected async Task<TResponse> ExecutePatchAsync<TResponse, TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength,
            JsonSerializerSettings jsonSerializerSettings = null
            )
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePatchAsync<TResponse, TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                jsonSerializerSettings: jsonSerializerSettings
            );

            return result;
        }

        protected async Task<string> ExecutePutchReturnStringAsync<TRequest>(string path,
            TRequest request,
            string callerMemberName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = HttpGlossary.TimeoutInMilliseconds,
            string overrieApiSchemeAndHost = null,
            long maxResponseContentLength = HttpGlossary.MaxResponseContentLength,
            JsonSerializerSettings jsonSerializerSettings = null)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequenceAndContext(headerValues, callerMemberName);

            var url = BuildUrlBySchemeAndHostAndPath(overrieApiSchemeAndHost: overrieApiSchemeAndHost, path: path);

            var result = await HttpJsonHelperService.ExecutePatchReturnStringAsync<TRequest>(
                url: url,
                request: request,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues,
                timeoutInMilliseconds: timeoutInMilliseconds,
                maxResponseContentLength: maxResponseContentLength,
                jsonSerializerSettings: jsonSerializerSettings
            );

            return result;
        }

        #endregion


        protected string BuildUrlBySchemeAndHostAndPath(string overrieApiSchemeAndHost, string path)
        {
            if (string.IsNullOrEmpty(overrieApiSchemeAndHost) == true && string.IsNullOrEmpty(ApiSchemeAndHostConfigKey) == true)
                throw new InvalidOperationException("ApiSchemeAndHostConfigKey is empty");


            var schemeAndHost = string.IsNullOrEmpty(overrieApiSchemeAndHost) == false ? overrieApiSchemeAndHost : Configuration[ApiSchemeAndHostConfigKey];

            if (string.IsNullOrEmpty(schemeAndHost))
                throw new InvalidOperationException($"schemeAndHost is empty by key = {ApiSchemeAndHostConfigKey}");

            var url = HttpUtils.BuildUrl(schemeAndHost: schemeAndHost, urlPath: path);

            return url;
        }

        private List<KeyValuePair<string, string>> GetHeaderValuesWithCallSequenceAndContext(List<KeyValuePair<string, string>> headerValues, string methodName)
        {
            var ret = headerValues?.ToList() ?? new List<KeyValuePair<string, string>>();

            var callSequnce = GetCallSequence();

            var httpContext = HttpContextAccessor?.HttpContext;

            var applicationName = ApplicationContextSingleton.GetApplication(httpContext);

            if (string.IsNullOrEmpty(applicationName) == false)
            {
                ret.Add(new KeyValuePair<string, string>("caller.application", applicationName));
            }

            var environment = ApplicationContextSingleton.GetEnvironment(httpContext);

            if (string.IsNullOrEmpty(environment) == false)
            {
                ret.Add(new KeyValuePair<string, string>("caller.environment", environment));
            }

            var customer = ApplicationContextSingleton.GetCustomer(httpContext);

            if (string.IsNullOrEmpty(customer) == false)
            {
                ret.Add(new KeyValuePair<string, string>("caller.customer", customer));
            }

            var version = ApplicationContextSingleton.GetVersion(httpContext);

            if (string.IsNullOrEmpty(version) == false)
            {
                ret.Add(new KeyValuePair<string, string>("caller.version", version));
            }

            var actionName = methodName;

            if (HttpContextAccessor.HttpContext != null)
                actionName = HttpContextAccessor.HttpContext.Request?.Path;

            callSequnce.Add(new CallSequenceHeaderModel { Application = applicationName, Action = actionName });

            var strCallSequence = JsonUtils.Serialize(callSequnce);

            ret.Add(new KeyValuePair<string, string>(CallSequenceKey, strCallSequence));

            var callSequenceId = GetCallSequenceIdFromContextItemsOrGenerateNew();
            if (string.IsNullOrEmpty(callSequenceId) == false)
            {
                ret.Add(new KeyValuePair<string, string>(CallSequenceIdKey, callSequenceId));
            }
            
            ret.SetXForwardedForHeader(httpContext);
            ret.SetXForwardedHostHeader(httpContext);

            return ret;
        }

        private List<CallSequenceHeaderModel> GetCallSequence()
        {
            var ret = new List<CallSequenceHeaderModel>();

            var isCallSequenceExists = HttpContextAccessor?.HttpContext?.Request.Headers.TryGetValue(CallSequenceKey, out var callSequence);
            if (isCallSequenceExists != true) 
                return ret;

            var iCallSequenceValues = callSequence.ToList();

            foreach (var iCallSequenceValue in iCallSequenceValues)
            {
                var methods = JsonUtils.Deserialize<List<CallSequenceHeaderModel>>(iCallSequenceValue);

                if (methods.Any() == false)
                {
                    continue;
                }

                ret.AddRange(methods);
            }

            return ret;
        }

        private string GetCallSequenceIdFromContextItemsOrGenerateNew()
        {
            var contextItems = HttpContextAccessor?.HttpContext?.Items;

            if (contextItems == null || contextItems.ContainsKey(CallSequenceIdKey) == false)
            {
                return Guid.NewGuid().ToString();
            }

            var ret = contextItems[CallSequenceIdKey];
            if (ret == null)
                return Guid.NewGuid().ToString();

            return ret.ToString();
        }

        class CallSequenceHeaderModel
        {
            public string Application { get; set; }

            public string Action { get; set; }
        }
    }
}
