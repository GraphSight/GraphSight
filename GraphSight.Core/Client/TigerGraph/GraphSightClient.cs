using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    public class GraphSightClient
    {
        TigerGraphAPIClient _apiClient; 
        private Action<Exception> _onErrorAction;
        private Action _onServiceStatusIsDownAction;

        private static readonly int _DEFAULT_RETRIES = 3;
        private static readonly int _DEFAULT_GET_TIMEOUT = 10;
        private static readonly int _DEFAULT_POST_TIMEOUT = 30;

        private string _token;

        public GraphSightClient() {
            _apiClient = new TigerGraphAPIClient();
            _apiClient.SetDefaultGetPolicy(_DEFAULT_GET_TIMEOUT);
            _apiClient.SetDefaultPostPolicy(_DEFAULT_POST_TIMEOUT);
            _apiClient.SetMaxRetryPolicy(_DEFAULT_RETRIES);
            _apiClient.SetCircuitBreakerPolicy(); 
        }

        public GraphSightClient(string username, string password, string URI, string secret, string graphName) {
            Credentials credentials = new Credentials()
            {
                Username = username,
                Password = password,
                URI = URI,
                Secret = secret,
                GraphName = graphName
            };

            _apiClient.SetCredentials(credentials);
            _apiClient
                .Configure(
                    baseURI: URI,
                    maxRetries: _DEFAULT_RETRIES,
                    httpGetTimeout: _DEFAULT_GET_TIMEOUT,
                    httpPostTimeout: _DEFAULT_POST_TIMEOUT,
                    onRetry: () => Console.WriteLine("retry"));
        }

        #region public

        public GraphSightClient SetUsername(string username) {
            _apiClient.SetUsername(username);
            return this; 
        }
        public GraphSightClient SetPassword(string password)
        {
            _apiClient.SetPassword(password); 
            return this;
        }
        public GraphSightClient SetURI(string uri)
        {
            _apiClient.SetURI(uri); 
            return this;
        }
        public GraphSightClient SetSecret(string secret)
        {
            _apiClient.SetSecret(secret);
            return this;
        }
        public GraphSightClient SetGraphName(string graphName) 
        {
            _apiClient.SetGraphName(graphName);
            return this; 
        }
        public GraphSightClient SetCustomErrorHandler(Action<Exception> action)
        { 
            _onErrorAction = action;
            _apiClient.SetCircuitBreakerPolicy(action); 
            return this; 
        }
        public GraphSightClient SetCustomServiceStatusIsDownAction(Action action)
        {
            _onServiceStatusIsDownAction = action;
            return this; 
        }
        public GraphSightClient WithMaxRetries(int maxRetries)
        {
            _apiClient.SetMaxRetryPolicy(maxRetries);
            return this; 
        }
        public GraphSightClient WithHttpGetTimeout(int httpGetTimeout) 
        {
            _apiClient.SetDefaultGetPolicy(httpGetTimeout);
            return this; 
        }
        public GraphSightClient WithHttpPostTimeout(int httpPostTimeout) 
        {
            _apiClient.SetDefaultPostPolicy(httpPostTimeout);
            return this; 
        } 

        public void BeginTracking(Guid sessionID = new Guid()) {
            throw new NotImplementedException();
        }

        public async Task<string> PingServerAsync() =>
            await CallAPI(() => { return _apiClient.PingServerAsync(); });
         
        public string PingServer() => PingServerAsync().Result;

        public async Task<string> RequestTokenAsync() =>
            await CallAPI(() => { return _apiClient.RequestTokenAsync(); });
        public string RequestToken() => RequestTokenAsync().Result;


        #endregion

        #region private

        /// <summary>
        /// Call API is a wrapper for any action calling an APIclient operation. 
        /// This method tries the connection in order to execute user-set error handling delegates. 
        /// </summary>
        /// <param name="apiCall">API action to be called. </param>
        private async void CallAPI(Action apiCall)
        {
            ValidateCredentials(); 
            await Task.Run(() => apiCall());
        }

        /// <summary>
        ///  Call API is a wrapper for any action calling an APIclient operation. 
        /// This method tries the connection in order to execute user-set error handling delegates. 
        /// Function call must contain a return to utilize this method. 
        /// </summary>
        /// <typeparam name="Task<T>">Expected Return Type</typeparam>
        /// <param name="apiCall">API action to be called.</param>
        /// <returns>Task of return type</returns>
        private async Task<T> CallAPI<T>(Func<Task<T>> apiCall)
        {
            ValidateCredentials();
            return await apiCall();
        }

        private bool ResponseIsServerStatusError(Exception ex)
        {
            return false; 
        }

        private void ValidateCredentials()
        {
            _apiClient.ValidateCredentials();
        }

        private void GetNewTokenIfNotSetAsync()
        {
            if (_token == null) 
                _apiClient.RequestTokenAsync().Wait();
        }

        private void CheckGraphName() 
        {
            if (String.IsNullOrEmpty(_apiClient.GetCredentials().GraphName))
                throw new Exception("A graph name must be supplied to the graph client in order to call this function.");
        }
        #endregion
    }
}
