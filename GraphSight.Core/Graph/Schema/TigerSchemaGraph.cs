using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph
{
    public class TigerSchemaGraph
    {
        public string Name { get; private set; }
        public Dictionary<string, TigerSchemaVertex> Vertices { get; private set; }
        public Dictionary<string, TigerSchemaEdge> Edges { get; private set; }

        private GraphOptions _options;

        public TigerSchemaGraph(string name, GraphOptions graphOptions = null)
        {
            _options = graphOptions;

            Name = name;
            Vertices = new Dictionary<string, TigerSchemaVertex>();
            Edges = new Dictionary<string, TigerSchemaEdge>();
        }

        #region PublicMethods
        public void AddVertex(TigerSchemaVertex vertex) => Vertices.Add(vertex.Name, vertex); 
        public void AddEdge(TigerSchemaEdge edge) => Edges.Add(edge.Name, edge);
        public void ClearVertices() => Vertices.Clear();
        public void ClearEdges() => Edges.Clear();

        public string GetGraphQuery() => SchemaQueryBuilder.CreateGraphQuery(this);
        #endregion

        #region InternalMethods
        internal void UpdateVertex(TigerSchemaVertex vertex)
        {
            if (Vertices.ContainsKey(vertex.Name))
                Vertices[vertex.Name] = vertex;
            else
                Vertices.Add(vertex.Name, vertex);
        }
        internal void UpdateEdge(TigerSchemaEdge edge)
        {
            if (Edges.ContainsKey(edge.Name))
                Edges[edge.Name] = edge;
            else
                Edges.Add(edge.Name, edge); 
        }
        #endregion
    }
}

