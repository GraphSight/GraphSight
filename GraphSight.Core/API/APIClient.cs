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
        protected GraphSightClient _graphSightClient;

        internal void Configure(string baseURI, int maxRetries, int httpGetTimeout, int httpPostTimeout)
        {
            this.SetMaxRetryPolicy(maxRetries);
            this.SetDefaultGetPolicy(httpGetTimeout);
            this.SetDefaultPostPolicy(httpPostTimeout);

            if (baseURI == null) return;

            Uri validUri = null;
            Uri.TryCreate(baseURI, UriKind.Absolute, out validUri);

            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", baseURI, 443, String.Empty).Uri;
        }

        protected GraphSightClient GetGraphSightClient() => _graphSightClient;

        public void SetMaxRetryPolicy(int maxRetries)
        {
            _HTTPRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .RetryAsync(maxRetries);
        }

        public void SetDefaultGetPolicy(int GET_timeout)
        {
            _HTTPGetPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(GET_timeout));
        }

        public void SetDefaultPostPolicy(int POST_timeout)
        {
            _HTTPPostPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(POST_timeout));
        }

        protected AsyncTimeoutPolicy<HttpResponseMessage> GetPolicy(HttpMethod httpMethod) => httpMethod == HttpMethod.Get ? _HTTPGetPolicy : _HTTPPostPolicy;

        protected async Task<string> HttpGetAsync(string endpoint, int port = 14240)
        {

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.GetAsync($"{_httpClient.BaseAddress.ToString().WithoutTrailingSlash()}:{port}{endpoint}", token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> HttpPostAsync(string endpoint, Dictionary<string, string> body, int port = 9000)
        {
            var content = new StringContent(body.FromDictionaryToJson(), Encoding.UTF8, "application/json");

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.PostAsync($"{_httpClient.BaseAddress.ToString().WithoutTrailingSlash()}:{port}{endpoint}", content, token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
