using System;

namespace GraphSight.Core
{
    internal interface IGraphSightClient
    {
        void GenerateSchemaIfNotExists();
        void Validate();
        void SetCustomErrorHandler(Action action);
        void SetCustomServiceStatusIsDownAction(Action action);
        void BeginTracking(Guid sessionID); 
    }
}