using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    public class GraphSightClient : IGraphSightClient
    {
        private TigerCredentials _credentials;
        private Action<Exception> _onErrorAction;
        private Action _onServiceStatusIsDownAction;

        private static readonly int _DEFAULT_RETRIES = 3;
        private static readonly int _DEFAULT_GET_TIMEOUT = 10;
        private static readonly int _DEFAULT_POST_TIMEOUT = 30;

        delegate Task<string> APICaller();
        public GraphSightClient() {
            _credentials = new TigerCredentials();

            ConfigureAPIClient();
        }

        public GraphSightClient(string username, string password, string URI, string secret) {
            _credentials = new TigerCredentials()
            {
                Username = username,
                Password = password,
                URI = URI,
                Secret = secret
            };

            TigerGraphAPIClient.Instance.SetCredentials(_credentials);
            TigerGraphAPIClient.Instance
                .Configure(
                    baseURI: URI,
                    maxRetries: _DEFAULT_RETRIES,
                    httpGetTimeout: _DEFAULT_GET_TIMEOUT,
                    httpPostTimeout: _DEFAULT_POST_TIMEOUT);
        }

        #region public

        public GraphSightClient SetUsername(string username) {
            _credentials.Username = username;
            ConfigureAPIClient();
            return this; 
        }
        public GraphSightClient SetPassword(string password)
        {
            _credentials.Password = password;
            ConfigureAPIClient();
            return this;
        }
        public GraphSightClient SetURI(string uri)
        {
            _credentials.URI = uri;
            ConfigureAPIClient();
            return this;
        }
        public GraphSightClient SetSecret(string secret)
        {
            _credentials.Secret = secret;
            ConfigureAPIClient();
            return this;
        }

        public void SetCustomErrorHandler(Action<Exception> action) => _onErrorAction = action;
        public void SetCustomServiceStatusIsDownAction(Action action) => _onServiceStatusIsDownAction = action;


        public GraphSightClient WithMaxRetries(int maxRetries)
        {
            TigerGraphAPIClient.Instance.SetMaxRetryPolicy(maxRetries);
            return this; 
        }
        public GraphSightClient WithHttpGetTimeout(int httpGetTimeout) 
        {
            TigerGraphAPIClient.Instance.SetDefaultGetPolicy(httpGetTimeout);
            return this; 
        }
        public GraphSightClient WithHttpPostTimeout(int httpPostTimeout) 
        {
            TigerGraphAPIClient.Instance.SetDefaultPostPolicy(httpPostTimeout);
            return this; 
        } 

        public void BeginTracking(Guid sessionID = new Guid()) {
            throw new NotImplementedException();
        }

        public Task<string> PingServerAsync() => 
            CallAPI(() => { return TigerGraphAPIClient.Instance.PingServerAsync(); });
        public string PingServer() => PingServerAsync().Result; 
        
        public void GenerateSchemaIfNotExists()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Internal
        internal TigerCredentials GetCredentials() => _credentials;
        internal void CallCustomErrorHandlerDelegate(Exception ex) => _onErrorAction?.Invoke(ex);
        internal void CallServiceStatusIsDownDelegate() => _onServiceStatusIsDownAction?.Invoke();

        #endregion

        #region private

        private void ConfigureAPIClient()
        {

            if (_credentials == null) return;

            TigerGraphAPIClient.Instance.SetCredentials(_credentials);
            TigerGraphAPIClient.Instance
                .Configure(
                    baseURI: _credentials.URI,
                    maxRetries: _DEFAULT_RETRIES,
                    httpGetTimeout: _DEFAULT_GET_TIMEOUT,
                    httpPostTimeout: _DEFAULT_POST_TIMEOUT);
        }

        /// <summary>
        /// Call API is a wrapper for any action calling an APIclient operation. 
        /// This method tries the connection in order to execute user-set error handling delegates. 
        /// </summary>
        /// <param name="apiCall">API action to be called. </param>
        private void CallAPI(Action apiCall)
        {
            ValidateCredentials(); 

            try
            {
                apiCall();
            }
            catch (Exception ex)
            {
                CallErrorDelegates(ex);
            }
        }

        /// <summary>
        ///  Call API is a wrapper for any action calling an APIclient operation. 
        /// This method tries the connection in order to execute user-set error handling delegates. 
        /// Function call must contain a return to utilize this method. 
        /// </summary>
        /// <typeparam name="T">Expected Return Type</typeparam>
        /// <param name="apiCall">API action to be called.</param>
        /// <returns></returns>
        private T CallAPI<T>(Func<T> apiCall)
        {
            ValidateCredentials(); 

            try
            {
                return apiCall();
            }
            catch (Exception ex)
            {
                CallErrorDelegates(ex);
            }
            return default; 
        }

        private void CallErrorDelegates(Exception ex)
        {
            if (ResponseIsServerStatusError(ex))
                CallServiceStatusIsDownDelegate();
            else if (_onErrorAction == null)
                CallCustomErrorHandlerDelegate(ex);
            else throw ex;
        }

        private bool ResponseIsServerStatusError(Exception ex)
        {
            return false; 
        }

        private void ValidateCredentials()
        {
            if (String.IsNullOrEmpty(_credentials.Username))
                throw new Exception("GraphSight Client requires a username.");
            if (String.IsNullOrEmpty(_credentials.Password))
                throw new Exception("GraphSight Client requires a password.");
            if (String.IsNullOrEmpty(_credentials.URI))
                throw new Exception("GraphSight Client requires a URI of the server you want to connect to.");
            if (String.IsNullOrEmpty(_credentials.Secret))
                throw new Exception("GraphSight Client requires a secret token. " +
                    "You can obtain the token by following instructions here: https://docs.tigergraph.com/tigergraph-server/current/user-access/managing-credentials");
        }



        #endregion
    }
}
