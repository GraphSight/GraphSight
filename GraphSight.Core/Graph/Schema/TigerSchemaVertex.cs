using GraphSight.Core.Enums.TigerGraph;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    public class TigerSchemaVertex
    {
        public string Name { get; private set; }
        public string PrimaryIdName { get; private set; }
        public PrimaryIDTypes PrimaryIdType { get; private set; }
        public List<TigerSchemaAttribute> Attributes { get; private set; } = new List<TigerSchemaAttribute>();

        private TigerSchemaVertex() { }
        public TigerSchemaVertex(string name, string primaryIdName, PrimaryIDTypes primaryIdType)
        {
            Name = name;
            PrimaryIdName = primaryIdName;
            PrimaryIdType = primaryIdType; 
        }

        public void AddAttribute(TigerSchemaAttribute attribute) => Attributes.Add(attribute);

        public void ClearAtrributes() => Attributes.Clear();

    }

    
}
