using System;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerEdge
    {
        public string Name { get; private set; }
        public List<SourceTargetPair> SourceTargetPairs { get; private set; } = new List<SourceTargetPair>();
        public List<TigerAttribute> Attributes { get; private set; } = new List<TigerAttribute>();

        private TigerEdge()
        {
        }

        internal TigerEdge(string name)
        {
            Name = name;
        }

        internal void AddTargetPair(TigerSchemaVertex fromVertex, TigerSchemaVertex toVertex) 
            => SourceTargetPairs.Add(new SourceTargetPair(fromVertex, toVertex));
        internal void AddAttribute(TigerAttribute attribute) => Attributes.Add(attribute);
        internal void ClearAtrributes() => Attributes.Clear();
    }
}
