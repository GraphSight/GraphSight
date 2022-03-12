using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace GraphSight.Core.API
{
    internal sealed class APIClient : IApiClientBuilder
    {
        private static readonly Lazy<APIClient> lazy
             = new Lazy<APIClient>(() => new APIClient());

        public static APIClient Instance => lazy.Value;

        private APIClient() { }

        private string _URI;
        private string _username;
        private string _password;
        private readonly HttpClient _httpClient = new HttpClient();
        public void SetURI(string URI) => _URI = URI;
        public void SetUsername(string username) => _username = username;
        public void SetPassword(string password) => _password = password;

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
