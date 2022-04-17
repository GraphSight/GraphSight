using GraphSight.Core.Enums.TigerGraph;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerSchemaVertex
    {
        public string Name { get; private set; }
        public string PrimaryIdName { get; private set; }
        public PrimaryIDTypes PrimaryIdType { get; private set; }
        public object PrimaryId { get; private set; }
        public List<TigerSchemaAttribute> Attributes { get; private set; } = new List<TigerSchemaAttribute>();

        private TigerSchemaVertex() { }
        internal TigerSchemaVertex(string name, string primaryIdName, PrimaryIDTypes primaryIdType)
        {
            Name = name;
            PrimaryId = primaryIdName;
            PrimaryIdType = primaryIdType; 
        }

        internal void AddAttribute(TigerSchemaAttribute attribute) => Attributes.Add(attribute);

        internal void ClearAtrributes() => Attributes.Clear();

    }

    
}
