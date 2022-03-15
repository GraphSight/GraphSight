using GraphSight.Core.Enums.TigerGraph;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    internal class TigerVertex
    {
        public string Name { get; set; }
        public string PrimaryId { get; set; }
        public PrimaryIDTypes PrimaryIdType { get; set; }
        public List<TigerVertexAttribute> Attributes { get; set; }

        public TigerVertex()
        {
        }

        public TigerVertex(string name,
            string primaryId,
            PrimaryIDTypes primaryIdType,
            List<TigerVertexAttribute> attributes)
        {
            Name = name;
            PrimaryId = primaryId;
            PrimaryIdType = primaryIdType;
            Attributes = attributes;
        }
    }
}
