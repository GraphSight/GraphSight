using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    internal abstract class APIClient : IApiClient
    {
        public int MaxRetries { get; set; } = 1;
        public int GetTimeout { get; set; }
        public int PostTimeout { get; set; }

        private AsyncTimeoutPolicy<HttpResponseMessage> _HTTPGetPolicy;
        private AsyncTimeoutPolicy<HttpResponseMessage> _HTTPPostPolicy;
        private AsyncRetryPolicy<HttpResponseMessage> _HTTPRetryPolicy;
        private AsyncCircuitBreakerPolicy _HTTPCircuitBreakerPolicy; 

        protected readonly HttpClient _httpClient = new HttpClient();

        internal void Configure(string baseURI, int maxRetries, int httpGetTimeout, int httpPostTimeout, Action<Exception> onError = null, Action onRetry = null)
        {
            MaxRetries = maxRetries;
            GetTimeout = httpGetTimeout;
            PostTimeout = httpPostTimeout; 

            SetDefaultGetPolicy(httpGetTimeout);
            SetDefaultPostPolicy(httpPostTimeout);
            SetMaxRetryPolicy(maxRetries, onRetry);
            SetCircuitBreakerPolicy(onError);

            SetURI(baseURI);
        }

        internal void SetMaxRetryPolicy(int maxRetries, Action onRetry = null)
        {
            MaxRetries = maxRetries; 

            //if (onRetry == null) 
            //    onRetry = () => { onRetry(); };

            _HTTPRetryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<Exception>() 
                .RetryAsync(maxRetries, (ex, retryCount) => {
                    if (onRetry != null) onRetry(); 
                });
        }

        internal void SetCircuitBreakerPolicy(Action<Exception> onError = null)
        {
            //if (onError == null)
            //    onError = (Exception) => { };

            _HTTPCircuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 1,
                    durationOfBreak: TimeSpan.FromSeconds(2),
                    onBreak: (ex, breakDelay) =>
                    {
                        if(onError != null) onError(ex); 
                    },
                    onReset: () => { },
                    onHalfOpen: () => { }
                );

        }

        internal void SetDefaultGetPolicy(int GET_timeout) {
            GetTimeout = GET_timeout; 
            _HTTPGetPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(GET_timeout));           
        }

        internal void SetDefaultPostPolicy(int POST_timeout) {
            PostTimeout = POST_timeout;
            _HTTPPostPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(POST_timeout));
        }

        protected AsyncTimeoutPolicy<HttpResponseMessage> GetPolicy(HttpMethod httpMethod) => httpMethod == HttpMethod.Get ? _HTTPGetPolicy : _HTTPPostPolicy;

        protected void SetBasicAuthentication(string user, string pass) 
        {
            var authenticationString = $"{user}:{pass}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
        }

        protected void SetTokenAuthentication(string token) 
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected async Task<string> HttpGetAsync(string endpoint, int port)
        {

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");
            

            HttpResponseMessage response = await
             _HTTPCircuitBreakerPolicy.ExecuteAsync(() =>
                _HTTPRetryPolicy.ExecuteAsync(() =>
                    _HTTPPostPolicy.ExecuteAsync(async token =>
                        await _httpClient.GetAsync(GetEnpointRequestAddress(endpoint, port), token), CancellationToken.None)));

            //response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> HttpPostAsync(string endpoint, Dictionary<string, string> body, int port)
        {
            var content = new StringContent(body.FromDictionaryToJson(), Encoding.UTF8, "application/json");

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            HttpResponseMessage response = await _httpClient.PostAsync(GetEnpointRequestAddress(endpoint, port), content);

            //TODO: Polly is currently disabled until better error handling is implemented. 

            //HttpResponseMessage response = await
            //_HTTPCircuitBreakerPolicy.ExecuteAsync(() => 
            //    _HTTPRetryPolicy.ExecuteAsync(() =>
            //        _HTTPPostPolicy.ExecuteAsync(async token =>
            //            await _httpClient.PostAsync(GetEnpointRequestAddress(endpoint, port), content, token), CancellationToken.None)));

            //response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<string> HttpPostAsync(string endpoint, string body, int port)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(body);
            var byteContent = new ByteArrayContent(buffer);

            if (_httpClient == null)
                throw new ArgumentNullException("HTTP client is not set.");

            HttpResponseMessage response = await _httpClient.PostAsync(GetEnpointRequestAddress(endpoint, port), byteContent);

            //TODO: Polly is currently disabled until better error handling is implemented. 

            //HttpResponseMessage response = await
            //_HTTPCircuitBreakerPolicy.ExecuteAsync(() => 
            //    _HTTPRetryPolicy.ExecuteAsync(() =>
            //        _HTTPPostPolicy.ExecuteAsync(async token =>
            //            await _httpClient.PostAsync(GetEnpointRequestAddress(endpoint, port), content, token), CancellationToken.None)));

            //response.EnsureSuccessStatusCode();
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

            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", baseURI, 443, String.Empty).Uri;
        }

    }
}
