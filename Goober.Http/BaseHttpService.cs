﻿using Goober.Http.Services;
using Goober.Http.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Goober.Http
{
    public abstract class BaseHttpService
    {
        #region fields

        protected abstract string ApiSchemeAndHostConfigKey { get; set; }

        private const string CallSequenceKey = "g-callsec";

        protected readonly IHttpJsonHelperService HttpJsonHelperService;
        protected readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IHostEnvironment _hostEnvironment;
        protected readonly IConfiguration Configuration;
        protected readonly string AssemblyName;

        #endregion

        #region ctor

        protected BaseHttpService(IConfiguration configuration, IHttpJsonHelperService httpJsonHelperService,
            IHttpContextAccessor httpContextAccessor,
            IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HttpJsonHelperService = httpJsonHelperService;
            HttpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
            AssemblyName = _hostEnvironment.ApplicationName;
        }

        #endregion

        protected async Task<TResponse> ExecuteGetAsync<TResponse>(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMethodName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = 12000)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequence(headerValues, callerMethodName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(path);

            var result = await HttpJsonHelperService.ExecuteGetAsync<TResponse>(
                urlWithoutQueryParameters: urlWithoutQueryParameters, 
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: newHeaderValues);

            return result;
        }

        protected async Task<string> ExecuteGetStringAsync(string path,
            List<KeyValuePair<string, string>> queryParameters,
            string callerMethodName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutMiliseconds = 12000)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequence(headerValues, callerMethodName);

            var urlWithoutQueryParameters = BuildUrlBySchemeAndHostAndPath(path);

            var result = await HttpJsonHelperService.ExecuteGetStringAsync(
                urlWithoutQueryParameters: urlWithoutQueryParameters,
                queryParameters: queryParameters,
                timeoutInMilliseconds: timeoutMiliseconds,
                authenticationHeaderValue: authenticationHeaderValue,
                headerValues: headerValues);

            return result;
        }

        protected async Task<TResponse> ExecutePostAsync<TResponse, TRequest>(string path,
            TRequest request,
            string callerMethodName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = 120000)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequence(headerValues, callerMethodName);

            var url = BuildUrlBySchemeAndHostAndPath(path);

            var result = await HttpJsonHelperService.ExecutePostAsync<TResponse, TRequest>(
                    url: url,
                    request: request,
                    authenticationHeaderValue: authenticationHeaderValue,
                    headerValues: newHeaderValues,
                    timeoutInMilliseconds: timeoutInMilliseconds
                );

            return result;
        }

        protected async Task<string> ExecutePostStringAsync<TRequest>(string path,
            TRequest request,
            string callerMethodName,
            AuthenticationHeaderValue authenticationHeaderValue = null,
            List<KeyValuePair<string, string>> headerValues = null,
            int timeoutInMilliseconds = 120000)
        {
            var newHeaderValues = GetHeaderValuesWithCallSequence(headerValues, callerMethodName);

            var url = BuildUrlBySchemeAndHostAndPath(path);

            var result = await HttpJsonHelperService.ExecutePostStringAsync<TRequest>(
                    url: url,
                    request: request,
                    authenticationHeaderValue: authenticationHeaderValue,
                    headerValues: headerValues,
                    timeoutInMilliseconds: timeoutInMilliseconds
                );

            return result;
        }

        protected string BuildUrlBySchemeAndHostAndPath(string path)
        {
            if (string.IsNullOrEmpty(ApiSchemeAndHostConfigKey) == true)
                throw new InvalidOperationException("ApiSchemeAndHostConfigKey is empty");

            var schemeAndHost = Configuration[ApiSchemeAndHostConfigKey];

            if (string.IsNullOrEmpty(schemeAndHost))
                throw new InvalidOperationException($"schemeAndHost is empty by key = {ApiSchemeAndHostConfigKey}");

            var url = HttpUtils.BuildUrl(schemeAndHost: schemeAndHost, urlPath: path);

            return url;
        }

        private List<KeyValuePair<string, string>> GetHeaderValuesWithCallSequence(List<KeyValuePair<string, string>> headerValues, string methodName)
        {
            var ret = headerValues?.ToList() ?? new List<KeyValuePair<string, string>>();

            var callSequnce = GetCallSequence();

            var actionName = HttpContextAccessor.HttpContext?.Request?.Path ?? methodName;

            var serviceAndMethodName = $"{AssemblyName}:{actionName}";

            callSequnce.Add(serviceAndMethodName);

            var strCallSequence = string.Join(";", callSequnce);

            ret.Add(new KeyValuePair<string, string>(CallSequenceKey, strCallSequence));

            return ret;
        }

        private List<string> GetCallSequence()
        {
            var ret = new List<string>();

            var isCallSequenceExists = HttpContextAccessor.HttpContext?.Request.Headers.TryGetValue(CallSequenceKey, out var callSequence);
            if (isCallSequenceExists == true)
            {
                var iCallSequenceValues = callSequence.ToList();

                foreach (var iCallSequenceValue in iCallSequenceValues)
                {
                    var methods = iCallSequenceValue.Split(";", StringSplitOptions.RemoveEmptyEntries);

                    if (methods.Any() == false)
                    {
                        continue;
                    }

                    ret.AddRange(methods);
                }
            }

            return ret.Distinct().ToList();
        }
    }
}
