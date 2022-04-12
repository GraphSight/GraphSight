using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal interface IEventTracker
    {
        void Track(IVertex fromVertex, string eventName, IVertex toVertex);
        void Track(IVertex fromVertex, IEdge edge, IVertex toVertex);
        void TrackEvent(IVertex fromVertex, string eventDescription);
        void TrackEvent(IVertex fromVertex, string eventID, string eventDescription);
        void TrackError(IVertex fromVertex, Exception exception, string description = "");
    }
}
