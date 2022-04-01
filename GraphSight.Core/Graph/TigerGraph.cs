using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph
{
    public class TigerGraph
    {
        public string Name { get; private set; }
        public List<TigerVertex> Vertices { get; private set; }
        public List<TigerEdge> Edges { get; private set; }

        private GraphOptions _options; 

        public TigerGraph(string name, GraphOptions graphOptions = null)
        {
            _options = graphOptions; 

            Name = name;
            Vertices = new List<TigerVertex>();
            Edges = new List<TigerEdge>();
        }

        #region PublicMethods
        public TigerGraph AddVertex(string name, string primaryIdName, PrimaryIDTypes primaryIdType) 
        {
            if(GetVertexByName(name) == null)
                Vertices.Add(new TigerVertex(name, primaryIdName, primaryIdType));

            return this; 
        }
        public TigerGraph AddVertexData(string name, object data, OperationCode? opCode = null) 
        {
            var vertex = GetVertexByName(name);
            if (vertex == null)
                throw new Exception("Cannot add data to a non-existent vertex.");
            
            //Todo: iterate object data using reflection to add attributes, unless the object is a primative type (check with converter)

            return this; 
        }

        public void AddEdge(string name) { }
        public void AddEdge(string name, TigerVertex fromVertex, TigerVertex toVertex) { }
        public void AddEdgeData(string name, object data, OperationCode? opCode = null) { }

        public void ClearVertexData(string name) { }
        public void ClearEdgeData(string name) { }

        private TigerVertex GetVertexByName(string name) => Vertices.Find(v => v.Name == name);
        #endregion

        #region PrivateMethods

        #endregion
    }
}
