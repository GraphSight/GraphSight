using GraphSight.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal interface IEventTracker
    {
        void TigerGraphDataInsert(IVertex fromVertex, string eventName, IVertex toVertex);
        void TigerGraphDataInsert(IVertex fromVertex, IEdge edge, IVertex toVertex);
        [Event]
        void TigerGraphTrackEvent(IVertex fromVertex, string eventDescription);
        [Event]
        void TigerGraphTrackEvent(IVertex fromVertex, string eventID, string eventDescription);
        [ErrorEvent]
        void TigerGraphTrackError(IVertex fromVertex, Exception exception, string description = "");
        [SequenceEvent]
        void TigerGraphTrackSequence(IVertex fromVertex, string sequenceID, string sequenceNumber, string description = "");
    }
}
 