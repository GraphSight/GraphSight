using System;

namespace GraphSight.Core
{
    internal interface IGraphSightClient
    {
        void GenerateSchemaIfNotExists();
        void SetCustomErrorHandler(Action<Exception> action);
        void SetCustomServiceStatusIsDownAction(Action action);
        void BeginTracking(Guid sessionID); 
    }
}