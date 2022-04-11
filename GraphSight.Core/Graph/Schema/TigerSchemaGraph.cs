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
        public List<TigerSchemaVertex> Vertices { get; private set; }
        public List<TigerSchemaEdge> Edges { get; private set; }

        private GraphOptions _options;

        public TigerSchemaGraph(string name, GraphOptions graphOptions = null)
        {
            _options = graphOptions;

            Name = name;
            Vertices = new List<TigerSchemaVertex>();
            Edges = new List<TigerSchemaEdge>();
        }

        #region PublicMethods
        public void AddVertex(TigerSchemaVertex vertex) => Vertices.Add(vertex); 
        public void AddEdge(TigerSchemaEdge edge) => Edges.Add(edge);
        public void ClearVertices() => Vertices.Clear();
        public void ClearEdges() => Edges.Clear(); 

        public string GetGraphQuery() => SchemaQueryBuilder.CreateGraphQuery(this); 
        #endregion

        #region PrivateMethods
        private TigerSchemaVertex GetVertexByName(string name) => Vertices.Find(v => v.Name == name);
        #endregion
        }
    }

