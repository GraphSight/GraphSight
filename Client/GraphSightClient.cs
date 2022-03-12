using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Client
{
    public class GraphSightClient : IGraphSightClientBuilder
    {
        private string _URI;
        private string _username;
        private string _password;
        private string _secret;
        private Action _onErrorAction; 

        public GraphSightClient() { }

        #region public
        public void SetURI(string URI) => _URI = URI;
        public void SetUsername(string username) => _username = username;
        public void SetPassword(string password) => _password = password;
        public void SetSecret(string secret) => _secret = secret;
        public void SetCustomErrorHandler(Action action) => _onErrorAction = action;
        public Guid GenerateSessionID() => Guid.NewGuid();

        public void GenerateSchemaIfNotExists()
        {
            throw new NotImplementedException();
        }


        public void Validate()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private

        #endregion
    }
}
