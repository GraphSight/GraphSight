using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph
{
    internal class Graph
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }

        public Graph()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
        }
    }
}
