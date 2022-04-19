using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.QueryBuilders
{
    internal static class SchemaQueryBuilder
    {
        public static string CreateGraphQuery(TigerSchemaGraph graph) 
        {
            StringBuilder sb = new StringBuilder();

            foreach (var vertex in graph.Vertices) 
                sb.AppendLine(CreateVertexQuery(vertex.Value));

            foreach (var edge in graph.Edges)
                sb.AppendLine(CreateEdgeQuery(edge.Value));

            sb.AppendLine($"CREATE GRAPH {graph.Name}(*)");

            return sb.ToString(); 
        }

        public static string CreateVertexQuery(TigerSchemaVertex vertex) {

            string primaryID = string.IsNullOrEmpty(vertex.PrimaryIdName) ? "" : $"PRIMARY_ID {vertex.PrimaryIdName}";
            if (vertex.Attributes.Count > 0) primaryID += ",";

            string attributes = GetAttributesAsString(vertex.Attributes); 

            return $"CREATE VERTEX {vertex.Name} ({primaryID}{attributes})";
        }

        public static string CreateEdgeQuery(TigerSchemaEdge edge) 
        {
            string directed = edge.IsDirected ? "DIRECTED" : "UNDIRECTED";

            string attributes = GetAttributesAsString(edge.Attributes);

            StringBuilder sb = new StringBuilder();
            foreach (var sourceValuePair in edge.sourceTargetPairs)
            {
                sb.AppendLine($"CREATE {directed} EDGE {edge.Name} (FROM {sourceValuePair.FromVertex.Name}, TO {sourceValuePair.ToVertex.Name}, {attributes})");
                
                if (!string.IsNullOrEmpty(edge.ReverseEdge) && edge.IsDirected)
                    sb.Append($"WITH REVERSE EDGE = \"{edge.ReverseEdge}\"");
            }
            string query = sb.ToString();

            return query; 
        }

        public static string GetAttributesAsString(List<TigerSchemaAttribute> attributes) {

            StringBuilder sb = new StringBuilder(); 

            foreach (var attribute in attributes) {
                string defaultValue = "";

                if (attribute.DefaultValue != null) 
                { 
                    defaultValue = $" DEFAULT {attribute.DefaultValue.ToString()}";
                }

                sb.Append(attribute.Name)
                    .Append(" ")
                    .Append(attribute.Type.ToString())
                    .Append(defaultValue)
                    .Append(",");    
            }

            return sb.ToString().TrimEnd(','); 
        }




    }
}
