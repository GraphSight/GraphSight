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
        protected Credentials _credentials;
        internal async Task<string> PingServerAsync() => await HttpGetAsync(TigerAPIEndpoints.Ping);

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
            if (String.IsNullOrEmpty(_credentials.Username))
                throw new Exception("Client requires a username.");
            if (String.IsNullOrEmpty(_credentials.Password))
                throw new Exception("Client requires a password.");
            if (String.IsNullOrEmpty(_credentials.URI))
                throw new Exception("Client requires a URI of the server you want to connect to.");
            if (String.IsNullOrEmpty(_credentials.Secret))
                throw new Exception("Client requires a secret token. " +
                    "You can obtain the token by following instructions here: https://docs.tigergraph.com/tigergraph-server/current/user-access/managing-credentials");
        }


    }
}
