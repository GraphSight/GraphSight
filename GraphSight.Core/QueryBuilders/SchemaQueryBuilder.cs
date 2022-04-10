using GraphSight.Core.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.QueryBuilders
{
    internal static class SchemaQueryBuilder
    {
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

            string query =  $"CREATE {directed} EDGE {edge.Name} (FROM {edge.FromVertex}, TO {edge.ToVertex}, {attributes})";
           
            if(string.IsNullOrEmpty(edge.ReverseEdge) && edge.IsDirected) 
                query += $"WITH REVERSE EDGE = \"{edge.ReverseEdge}\"";

            return query; 
        }

        public static string GetAttributesAsString(List<TigerAttribute> attributes) {

            StringBuilder sb = new StringBuilder(); 

            foreach (var attribute in attributes) {
                sb.Append(attribute.Name)
                    .Append(" ")
                    .Append(attribute.Type.ToString())
                    .Append(",");    
            }

            return sb.ToString().TrimEnd(','); 
        }




    }
}
