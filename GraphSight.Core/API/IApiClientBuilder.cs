using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.API
{
    interface IApiClientBuilder
    {
        void SetURI(string URI);
        void SetUsername(string username);
        void SetPassword(string password);
        void SetSecret(string password);
    }
}
