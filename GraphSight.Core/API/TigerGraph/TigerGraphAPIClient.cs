using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    internal sealed class TigerGraphAPIClient : APIClient
    {
        private static readonly Lazy<TigerGraphAPIClient> lazy
            = new Lazy<TigerGraphAPIClient>(() => new TigerGraphAPIClient());
        public static TigerGraphAPIClient Instance => lazy.Value;

        public async Task<string> PingServerAsync() {

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            Task<string> pingServer = _httpClient.GetStringAsync(_httpClient.BaseAddress + "/ping");
            return await pingServer;
        }

        internal void SetCredentials(TigerCredentials credentials) => _credentials = credentials;
        internal void HandleServiceFault() => GetGraphSightClient()?.CallServiceStatusIsDownDelegate(); 

        private void ValidateCredentials() {
            if(_credentials == null) 
                throw new NullReferenceException("Cannot call API functions using null credentials.");
        }

        private async Task RequestTokenAsync() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Custom Delegate Invokation called if the TigerGraph api endpoint is currently not available. 
        /// </summary>
        
    }
}
