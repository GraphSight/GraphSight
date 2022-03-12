using System;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    internal class Edge
    {
        public string Name { get; set; }
        public bool IsDirected { get; set; }
        public bool IsReverseEdge { get; set; }
        public List<Tuple<Vertex, Vertex>> SourceTargetPairs { get; set; }
        public List<EdgeAttribute> Attributes { get; set; }

        public Edge()
        {
        }

        public Edge(string name,
            bool isDirected,
            bool isReverseEdge,
            List<Tuple<Vertex, Vertex>> sourceTargetPairs,
            List<EdgeAttribute> attributes)
        {
            Name = name;
            IsDirected = isDirected;
            IsReverseEdge = isReverseEdge;
            SourceTargetPairs = sourceTargetPairs;
            Attributes = attributes;
        }
    }
}
