using GraphSight.Core.Enums.TigerGraph;
using System.Collections.Generic;

namespace GraphSight.Core.Graph
{
    internal class Vertex
    {
        public string Name { get; set; }
        public string PrimaryId { get; set; }
        public PrimaryIDTypes PrimaryIdType { get; set; }
        public List<VertexAttribute> Attributes { get; set; }

        public Vertex()
        {
        }

        public Vertex(string name,
            string primaryId,
            PrimaryIDTypes primaryIdType,
            List<VertexAttribute> attributes)
        {
            Name = name;
            PrimaryId = primaryId;
            PrimaryIdType = primaryIdType;
            Attributes = attributes;
        }
    }
}
