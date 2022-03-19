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
        private static readonly int DEFAULT_PORT = 9000;
        private string _token; 
        protected Credentials _credentials;

        public TigerGraphAPIClient() {
            _credentials = new Credentials();
        }

        internal async Task<string> PingServerAsync() => await HttpGetAsync(TigerAPIEndpoints.Ping, 14240);
        internal async Task<string> RequestTokenAsync() =>
            await HttpPostAsync(TigerAPIEndpoints.RequestToken, GetCredentialBody(), DEFAULT_PORT);
        internal void SetCredentials(Credentials credentials)
        {
            _credentials = credentials;
            base.SetURI(_credentials.URI);
        }
        internal void SetUsername(string username) => _credentials.Username = username;
        internal void SetPassword(string password) => _credentials.Password = password;
        internal void SetSecret(string secret) => _credentials.Secret = secret;
        internal void SetURI(string uri) {
            _credentials.URI = uri; 
            base.SetURI(uri); 
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
