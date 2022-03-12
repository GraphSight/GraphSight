using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace GraphSight.Core.API
{
    internal abstract class APIClient : IApiClientBuilder
    {
        protected string _URI;
        protected string _username;
        protected string _password;
        protected string _secret;
        protected string _token;

        protected readonly HttpClient _httpClient = new HttpClient();
        public void SetURI(string URI) => _URI = URI;
        public void SetUsername(string username) => _username = username;
        public void SetPassword(string password) => _password = password;
        public void SetSecret(string secret) => _secret = secret;

        protected void Validate()
        {
            throw new NotImplementedException();
        }

        protected void GenerateToken() { }

    }
}
