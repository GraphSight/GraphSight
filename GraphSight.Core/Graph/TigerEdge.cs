using System;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    internal class TigerEdge
    {
        public string Name { get; set; }
        public bool IsDirected { get; set; }
        public bool IsReverseEdge { get; set; }
        public List<Tuple<TigerVertex, TigerVertex>> SourceTargetPairs { get; set; }
        public List<TigerEdgeAttribute> Attributes { get; set; }

        public TigerEdge()
        {
        }

        public TigerEdge(string name,
            bool isDirected,
            bool isReverseEdge,
            List<Tuple<TigerVertex, TigerVertex>> sourceTargetPairs,
            List<TigerEdgeAttribute> attributes)
        {
            Name = name;
            IsDirected = isDirected;
            IsReverseEdge = isReverseEdge;
            SourceTargetPairs = sourceTargetPairs;
            Attributes = attributes;
        }
    }
}
