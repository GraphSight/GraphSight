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
            TigerGraphAPIClient.Instance.ConstructBaseUri("/api");
        }

        #region public
        public void SetCustomErrorHandler(Action action) => _onErrorAction = action;
        public void SetCustomServiceStatusIsDownAction(Action action) => _onServiceStatusIsDownAction = action;

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
