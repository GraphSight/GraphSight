using System;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerSchemaEdge
    {
        public string Name { get; private set; }
        public bool IsDirected { get; private set; } = false;
        public string ReverseEdge { get; private set; }
        public List<SourceTargetPair> sourceTargetPairs { get; private set; } = new List<SourceTargetPair>();
        public List<TigerSchemaAttribute> Attributes { get; private set; } = new List<TigerSchemaAttribute>();

        private TigerSchemaEdge()
        {
        }

        public TigerSchemaEdge(string name,
            bool isDirected = false,
            string reverseEdge = null)
        {
            Name = name;
            IsDirected = isDirected;
            ReverseEdge = reverseEdge;
        }

        public void AddAttribute(TigerSchemaAttribute attribute) => Attributes.Add(attribute);
        public void AddSourceTargetPair(TigerSchemaVertex fromVertex, TigerSchemaVertex toVertex) 
            => sourceTargetPairs.Add(new SourceTargetPair(fromVertex, toVertex));
        public void ClearAtrributes() => Attributes.Clear();
        public void ClearSourceTargetPairs() => sourceTargetPairs.Clear();
    }

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
