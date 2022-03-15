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
        private Action _onErrorAction;
        private Action _onServiceStatusIsDownAction;

        private static readonly int _DEFAULT_RETRIES = 3;
        private static readonly int _DEFAULT_GET_TIMEOUT = 10;
        private static readonly int _DEFAULT_POST_TIMEOUT = 30;

        private GraphSightClient() { }

        public GraphSightClient(string username, string password, string URI, string secret) {
            _credentials = new TigerCredentials()
            {
                Username = username,
                Password = password,
                Domain = URI,
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
        public void SetCustomErrorHandler(Action action) => _onErrorAction = action;
        public void SetCustomServiceStatusIsDownAction(Action action) => _onServiceStatusIsDownAction = action;

        public void SetMaxRetries(int maxRetries) => TigerGraphAPIClient.Instance.SetMaxRetryPolicy(maxRetries);
        public void SetHttpGetTimeout(int httpGetTimeout) => TigerGraphAPIClient.Instance.SetDefaultGetPolicy(httpGetTimeout);
        public void SetHttpPostTimeout(int httpPostTimeout) => TigerGraphAPIClient.Instance.SetDefaultPostPolicy(httpPostTimeout);

        public void BeginTracking(Guid sessionID = new Guid()) {
            throw new NotImplementedException();
        }

        public string PingServer() => Task.Run(() => TigerGraphAPIClient.Instance.PingServerAsync()).Result; 

        public void GenerateSchemaIfNotExists()
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Internal
        internal TigerCredentials GetCredentials() => _credentials;
        internal void CallCustomErrorHandlerDelegate() => _onErrorAction?.Invoke();
        internal void CallServiceStatusIsDownDelegate() => _onServiceStatusIsDownAction?.Invoke(); 
        #endregion

        #region private

        #endregion
    }
}
