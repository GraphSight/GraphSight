using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.Core
{
    internal sealed class TigerGraphAPIClient : APIClient
    {
        private static readonly string TOKEN_LIFETIME = "100000";  //Specifies time before a token is reset within TigerGraph.
        private string _token; 
        protected TigerCredentials _credentials;

        private static readonly Lazy<TigerGraphAPIClient> lazy
            = new Lazy<TigerGraphAPIClient>(() => new TigerGraphAPIClient());
        public static TigerGraphAPIClient Instance => lazy.Value;

        internal async Task<string> PingServerAsync() => await HttpGetAsync(TigerAPIEndpoints.Ping); 
        internal void SetCredentials(TigerCredentials credentials) => _credentials = credentials;
        internal void HandleServiceFault() => GetGraphSightClient()?.CallServiceStatusIsDownDelegate(); 

        private void ValidateCredentials() {
            if(_credentials == null) 
                throw new NullReferenceException("Cannot call API functions using null credentials.");
        }
    }
}
