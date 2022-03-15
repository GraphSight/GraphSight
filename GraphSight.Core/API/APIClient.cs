using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GraphSight.Core;
using Polly;
using Polly.Timeout;
using Polly.Retry;
using System.Threading;

namespace GraphSight.Core
{
    internal abstract class APIClient : IApiClient
    {
        private AsyncTimeoutPolicy<HttpResponseMessage> _getPolicy;
        private AsyncTimeoutPolicy<HttpResponseMessage> _postPolicy;
        private AsyncRetryPolicy<HttpResponseMessage> _httpRetryPolicy;

        protected readonly HttpClient _httpClient = new HttpClient();
        protected GraphSightClient _graphSightClient;

        internal void Configure(string baseURI, int maxRetries, int httpGetTimeout, int httpPostTimeout)
        {

            Uri validUri = null;
            Uri.TryCreate(baseURI, UriKind.Absolute, out validUri);

            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", baseURI, 443, "").Uri;

            this.SetMaxRetryPolicy(maxRetries); 
            this.SetDefaultGetPolicy(httpGetTimeout);
            this.SetDefaultPostPolicy(httpPostTimeout);

        }

        protected GraphSightClient GetGraphSightClient() => _graphSightClient;

        protected void SetMaxRetryPolicy(int maxRetries)
        {
            _httpRetryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .Or<TimeoutRejectedException>()
                    .RetryAsync(maxRetries);
        }

        protected void SetDefaultGetPolicy(int GET_timeout) {
            _getPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(GET_timeout));           
        }

        protected void SetDefaultPostPolicy(int POST_timeout) {
            _postPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(POST_timeout));
        }

        protected AsyncTimeoutPolicy<HttpResponseMessage> GetPolicy(HttpMethod httpMethod) => httpMethod == HttpMethod.Get ? _getPolicy : _postPolicy;

        protected virtual async Task<string> HttpGetAsync(string endpoint) {

            if(_httpClient == null) 
                throw new ArgumentNullException("Http client is undeclared.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            HttpResponseMessage response = await
             _httpRetryPolicy.ExecuteAsync(() =>
             _postPolicy.ExecuteAsync(async token =>
             await _httpClient.GetAsync($"{_httpClient.BaseAddress}{endpoint}", token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected virtual async Task<string> HttpPostAsync(string endpoint, Dictionary<string, string> body)
        {
            var content = new FormUrlEncodedContent(body);

            if (_httpClient == null)
                throw new ArgumentNullException("Http client is undeclared.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _httpRetryPolicy.ExecuteAsync(() =>
             _postPolicy.ExecuteAsync(async token =>
             await _httpClient.PostAsync($"{_httpClient.BaseAddress}{endpoint}", content, token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }



    }
}
