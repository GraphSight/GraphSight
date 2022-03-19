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

        protected async Task<string> HttpGetAsync(string endpoint) {

            if(_httpClient == null) 
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            Console.WriteLine($"{_httpClient.BaseAddress}{endpoint}");

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.GetAsync($"{_httpClient.BaseAddress}{endpoint}", token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> HttpPostAsync(string endpoint, Dictionary<string, string> body)
        {
            var content = new FormUrlEncodedContent(body);

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            HttpResponseMessage response = await
             _HTTPRetryPolicy.ExecuteAsync(() =>
             _HTTPPostPolicy.ExecuteAsync(async token =>
             await _httpClient.PostAsync($"{_httpClient.BaseAddress}{endpoint}", content, token), CancellationToken.None));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected void SetURI(string baseURI)
        {
            if (baseURI == null) {
                _httpClient.BaseAddress = null;
                return; 
            }

            Uri validUri = null;
            Uri.TryCreate(baseURI, UriKind.Absolute, out validUri);

            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", baseURI, 443, String.Empty).Uri;
        }

    }
}
