using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GraphSight.Core;

namespace GraphSight.Core
{
    internal abstract class APIClient : IApiClient
    {
        protected ICredentials _credentials; 
        protected string _token;
        
        protected readonly HttpClient _httpClient = new HttpClient();
        protected GraphSightClient _graphSightClient;
        public void SetToken(string token) => _token = token;

        /// <summary>
        /// Builds the http client base address. User may input an incomplete URI, which will 
        /// be built via this method to create a valid URI. 
        /// </summary>
        /// <param name="apiDomain">Starting path of api in domain, such as "/api"</param>
        internal void ConstructBaseUri(string apiDomain = "")
        {

            Uri validUri = null;
            Uri.TryCreate(_credentials.Domain, UriKind.Absolute, out validUri);

            _httpClient.BaseAddress = validUri ?? new UriBuilder("https", _credentials.Domain, 443, apiDomain).Uri;

        }

        protected GraphSightClient GetGraphSightClient() => _graphSightClient; 
        protected void GenerateToken() { }


    }
}
