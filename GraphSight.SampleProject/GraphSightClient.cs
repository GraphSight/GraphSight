using GraphSight.Core;
using System;
            using System.Collections.Generic;
            using System.Text;
            using System.Threading.Tasks;

namespace GraphSight.Core
    {
        public class GraphSightClient : IEventTracker
        {
            TigerGraphAPIClient _apiClient;
            private Action<Exception> _onErrorAction;
            private Action _onServiceStatusIsDownAction;

            private static readonly int _DEFAULT_RETRIES = 3;
            private static readonly int _DEFAULT_GET_TIMEOUT = 10;
            private static readonly int _DEFAULT_POST_TIMEOUT = 30;

            private string _token;

            public GraphSightClient()
            {
                _apiClient = new TigerGraphAPIClient();
                _apiClient.SetDefaultGetPolicy(_DEFAULT_GET_TIMEOUT);
                _apiClient.SetDefaultPostPolicy(_DEFAULT_POST_TIMEOUT);
                _apiClient.SetMaxRetryPolicy(_DEFAULT_RETRIES);
                _apiClient.SetCircuitBreakerPolicy();
            }

            #region public

            #region Setters
            public GraphSightClient SetUsername(string username)
            {
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
            #endregion

            #region APICalls
            public async Task<string> PingServerAsync() =>
                await CallAPI(() => { return _apiClient.PingServerAsync(); });

            public string PingServer() => PingServerAsync().Result;

            public async Task<string> RequestTokenAsync() =>
                await CallAPI(() => { return _apiClient.RequestTokenAsync(); });
            public string RequestToken() => RequestTokenAsync().Result;

            public async Task<string> RunQueryAsync(string query) =>
                await CallAPI(() => { return _apiClient.QueryAsync(query); });
            public string RunQuery(string query) => RunQueryAsync(query).Result;

            public async Task<string> RunQueryAsync(string query, Dictionary<string, object> parameters) =>
                await CallAPI(() => { return _apiClient.QueryAsync(query, parameters); });
            public string RunQuery(string query, Dictionary<string, object> parameters) =>
                RunQueryAsync(query, parameters).Result;
            #endregion


            #region Tracking
            public void TigerGraphDataInsert(IVertex fromVertex, string eventName, IVertex toVertex)
            {
                //TODO: Stub
            }

            public void TigerGraphDataInsert(IVertex fromVertex, IEdge edge, IVertex toVertex)
            {
                //TODO: Stub
            }

            public void TigerGraphTrackEvent(IVertex fromVertex, string eventDescription)
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }

            public void TigerGraphTrackEvent(IVertex fromVertex, string eventID, string eventDescription)
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }

            public void TigerGraphTrackError(IVertex fromVertex, Exception exception, string description = ")
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }
            #endregion

            #endregion

            #region private

            /// <summary>
            /// Call API is a wrapper for any action calling an APIclient operation. 
            /// This method tries the connection in order to execute user-set error handling delegates. 
            /// </summary>
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

            }
            #endregion
        }
    }
