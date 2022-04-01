using System;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerEdge
    {
        public string Name { get; private set; }
        public bool IsDirected { get; private set; } = false;
        public bool IsReverseEdge { get; private set; } = false;
        public List<Tuple<TigerVertex, TigerVertex>> SourceTargetPairs { get; private set; } = new List<Tuple<TigerVertex, TigerVertex>>();
        public List<TigerAttribute> Attributes { get; private set; } = new List<TigerAttribute>();

        private TigerEdge()
        {
        }

        internal TigerEdge(string name,
            bool isDirected = false,
            bool isReverseEdge = false)
        {
            Name = name;
            IsDirected = isDirected;
            IsReverseEdge = isReverseEdge;
        }

        internal void AddTargetPair(TigerVertex fromVertex, TigerVertex toVertex) 
            => SourceTargetPairs.Add(new Tuple<TigerVertex, TigerVertex>(fromVertex, toVertex));
        internal void AddAttribute(TigerAttribute attribute) => Attributes.Add(attribute);
        internal void ClearAtrributes() => Attributes.Clear();
    }
}
