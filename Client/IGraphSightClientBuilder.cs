using System;

namespace GraphSight.Core.Client
{
    internal interface IGraphSightClientBuilder
    {
        void SetURI(string URI);
        void SetUsername(string username);
        void SetPassword(string password);
        void GenerateSchemaIfNotExists();
        void Validate();
        void SetCustomErrorHandler(Action action);
        Guid GenerateSessionID(); 
    }
}