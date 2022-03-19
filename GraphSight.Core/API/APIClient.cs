using GraphSight.Core.Extensions;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    internal abstract class APIClient : IApiClient
    {
        private AsyncTimeoutPolicy<HttpResponseMessage> _HTTPGetPolicy;
        private AsyncTimeoutPolicy<HttpResponseMessage> _HTTPPostPolicy;
        private AsyncRetryPolicy<HttpResponseMessage> _HTTPRetryPolicy;

        protected readonly HttpClient _httpClient = new HttpClient();

        internal void Configure(string baseURI, int maxRetries, int httpGetTimeout, int httpPostTimeout)
        {
            SetMaxRetryPolicy(maxRetries);
            SetDefaultGetPolicy(httpGetTimeout);
            SetDefaultPostPolicy(httpPostTimeout);

            SetURI(baseURI);
        }

        internal void SetMaxRetryPolicy(int maxRetries)
        {
            _HTTPRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .RetryAsync(maxRetries);
        }

        internal void SetDefaultGetPolicy(int GET_timeout) {
            _HTTPGetPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(GET_timeout));           
        }

        internal void SetDefaultPostPolicy(int POST_timeout) {
            _HTTPPostPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(POST_timeout));
        }

        protected AsyncTimeoutPolicy<HttpResponseMessage> GetPolicy(HttpMethod httpMethod) => httpMethod == HttpMethod.Get ? _HTTPGetPolicy : _HTTPPostPolicy;

        protected async Task<string> HttpGetAsync(string endpoint, int port)
        {

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.GetAsync(GetEnpointRequestAddress(endpoint, port), token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> HttpPostAsync(string endpoint, Dictionary<string, string> body, int port)
        {
            var content = new StringContent(body.FromDictionaryToJson(), Encoding.UTF8, "application/json");

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.PostAsync(GetEnpointRequestAddress(endpoint, port), content, token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private string GetEnpointRequestAddress(string endpoint, int port)
        {
            return $"{_httpClient.BaseAddress.ToString().WithoutTrailingSlash()}:{port}{endpoint}";
        }

        protected void SetURI(string baseURI)
        {
            if (baseURI == null) {
                _httpClient.BaseAddress = null;
                return; 
            }

            Uri validUri = null;
            Uri.TryCreate(baseURI, UriKind.Absolute, out validUri);
            new UriBuilder()
            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", baseURI, 443, String.Empty).Uri;
        }

    }
}
