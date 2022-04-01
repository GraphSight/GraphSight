using GraphSight.Core;
using Newtonsoft.Json.Linq;
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
        private static readonly string DEFAULT_TOKEN_LIFETIME = "100000";  //Specifies time before a token is reset within TigerGraph.
        private static readonly int DEFAULT_PORT = 9000;
        protected Credentials _credentials;
        private string _token; 

        public TigerGraphAPIClient() {
            _credentials = new Credentials();
        }
        public void SetCredentials(Credentials credentials)
        {
            _credentials = credentials;
            base.SetURI(_credentials.URI);
        }
        public void SetUsername(string username) => _credentials.Username = username;
        public void SetPassword(string password) => _credentials.Password = password;
        public void SetSecret(string secret) => _credentials.Secret = secret;
        public void SetGraphName(string graphName) => _credentials.GraphName = graphName;
        public void SetURI(string uri)
        {
            _credentials.URI = uri;
            base.SetURI(uri);
        }

        public async Task<string> PingServerAsync() => await HttpGetAsync(TigerAPIEndpoints.Ping, 14240);
        public async Task<string> RequestTokenAsync()
        {
            var body = GetCredentialBody();
            body.Add("lifetime", DEFAULT_TOKEN_LIFETIME);

            var result = await HttpPostAsync(TigerAPIEndpoints.RequestToken, body, DEFAULT_PORT);
            _token = JObject.Parse(result).GetValue("token").ToString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            
            return _token;
        }

        internal void ValidateCredentials()
        {
            if (String.IsNullOrEmpty(_credentials.URI))
                throw new Exception("Client requires a URI of the server you want to connect to.");

            if (String.IsNullOrEmpty(_credentials.Password)) { 
                if(String.IsNullOrEmpty(_credentials.Secret)) 
                    throw new Exception("Client requires either a username and password combination or a secret token." +
                        " See https://docs.tigergraph.com/tigergraph-server/current/user-access/managing-credentials");
            }
        }

        internal Credentials GetCredentials() => _credentials;

        private bool UserSecretIsSet()
        {
            return !String.IsNullOrEmpty(_credentials.Secret);
        }

        private bool UserPasswordComboAreSet() 
        {
            return !String.IsNullOrEmpty(_credentials.Username)
                && !String.IsNullOrEmpty(_credentials.Password);
        }

        private Dictionary<string, string> GetCredentialBody() {

            Dictionary<string, string> body = new Dictionary<string, string>();

            if (UserSecretIsSet()) 
            {
                body.Add("secret", _credentials.Secret);
            }
            else if (UserPasswordComboAreSet()) 
            {
                body.Add("username", _credentials.Secret);
                body.Add("password", _credentials.Secret);
            }

            return body; 
        }


    }
}
