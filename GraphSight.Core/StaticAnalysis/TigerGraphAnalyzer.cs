using GraphSight.Core.Graph;
using GraphSight.Core.GraphBuilders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    public static class TigerGraphAnalyzer
    {
        public static TigerSchemaGraph AnalyzeCodeAndGenerateSchema(string graphName, List<Assembly> assemblies = null) 
        {
            SchemaGenerator generator = new SchemaGenerator();
            return generator.GenerateSchema(graphName, assemblies);
        }

        public static string AnalyzeCodeAndGenerateSchemaAsQuery(string graphName, List<Assembly> assemblies = null)
        {
            SchemaGenerator generator = new SchemaGenerator();
            return generator.GenerateSchemaAsText(graphName, assemblies);
        }
    }
}
