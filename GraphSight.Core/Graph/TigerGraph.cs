using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph
{
    internal class TigerGraph
    {
        public List<TigerVertex> Vertices { get; set; }
        public List<TigerEdge> Edges { get; set; }

        public TigerGraph()
        {
            Vertices = new List<TigerVertex>();
            Edges = new List<TigerEdge>();
        }
    }
}
