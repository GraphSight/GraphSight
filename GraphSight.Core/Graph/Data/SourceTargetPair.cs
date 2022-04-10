using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph.Schema
{
    public class SourceTargetPair
    {
        public TigerSchemaVertex FromVertex { get; set; }
        public TigerSchemaVertex ToVertex { get; set; }

        public SourceTargetPair() { }
        public SourceTargetPair(TigerSchemaVertex fromVertex, TigerSchemaVertex toVertex)
        {
            FromVertex = fromVertex;
            ToVertex = toVertex;
        }
    }
}
