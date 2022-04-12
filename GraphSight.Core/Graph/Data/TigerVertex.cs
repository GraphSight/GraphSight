using GraphSight.Core.Enums.TigerGraph;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerVertex : IVertex
    {
        public string Name { get; private set; }
        public object PrimaryId { get; private set; }
        public List<TigerAttribute> Attributes { get; private set; } = new List<TigerAttribute>();

        private TigerVertex() { }
        internal TigerVertex(string name, string primaryIdName)
        {
            Name = name;
            PrimaryId = primaryIdName; 
        }

        internal void AddAttribute(TigerAttribute attribute) => Attributes.Add(attribute);

        internal void ClearAtrributes() => Attributes.Clear();

    }

    
}
