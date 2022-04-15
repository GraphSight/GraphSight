using GraphSight.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal interface IEventTracker
    {
        void Track(IVertex fromVertex, string eventName, IVertex toVertex);
        void Track(IVertex fromVertex, IEdge edge, IVertex toVertex);
        [Event]
        void TrackEvent(IVertex fromVertex, string eventDescription);
        [Event]
        void TrackEvent(IVertex fromVertex, string eventID, string eventDescription);
        [ErrorEvent]
        void TrackError(IVertex fromVertex, Exception exception, string description = "");
    }
}
