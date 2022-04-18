using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.Graph;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core.GraphBuilders
{
    /// <summary>
    /// This class is used to iterate a user's namespace to dynamically build a graph schema. 
    /// This allows for the return of a schema as a graph query or loading data directly into class objects without
    /// the user having to define definitions. 
    /// </summary>
    internal class SchemaGenerator
    {
        public bool HasErrorHandling { get; set; }
        public bool HasEventTracking { get; set; }

        private INamespaceAnalyzer _namespaceIterator;
        private IValueConverter _valueConverter;

        public SchemaGenerator() 
        {
            _namespaceIterator = new NamespaceAnalyzer();
            _valueConverter = new TigerValueConverter(); 

        }

        public TigerSchemaGraph GenerateSchema(string graphName, List<Assembly> assemblies = null)
        {
            NamespaceAnalyzer analyzer = new NamespaceAnalyzer(assemblies);

            var eventMethods = typeof(IEventTracker).GetMethods();

            TigerSchemaGraph graph = new TigerSchemaGraph(graphName);

            graph = AddVertices(graph, assemblies, analyzer);
            graph = AddEdges(graph, assemblies, analyzer);

            var dataInserts = analyzer.GetMethodInvocationsByName("TigerGraphDataInsert");
            var errorEvents = analyzer.GetMethodInvocationsByName("TigerGraphTrackError");
            var trackingEvents = analyzer.GetMethodInvocationsByName("TigerGraphTrackEvent");

            graph = AddSourceTargetPairsForDataInsertions(dataInserts, analyzer, graph);
            graph = AddSourceTargetPairsForErrors(dataInserts, analyzer, graph);
            graph = AddSourceTargetPairsForTrackingEvents(dataInserts, analyzer, graph);

            return graph;
        }

        public string GenerateSchemaAsText(string graphName, List<Assembly> assemblies = null)
        {
            return GenerateSchema(graphName, assemblies).GetGraphQuery();
        }

        private TigerSchemaGraph AddVertices(TigerSchemaGraph graph, List<Assembly> assemblies, NamespaceAnalyzer analyzer)
        {
            List<TigerSchemaVertex> vertices = new List<TigerSchemaVertex>(); 

            List<Type> vertexTypes = analyzer
                .GetCallerNamespaceTypesImplementingInterface<IVertex>()
                .Distinct()
                .ToList();

            foreach (Type vertexType in vertexTypes)
            {
                VertexName vertexName = (VertexName)Attribute.GetCustomAttribute(vertexType, typeof(VertexName));

                PropertyInfo primaryKeyProperty = GetAttributeProperties<PrimaryKey>(vertexType).FirstOrDefault();
                FieldInfo primaryKeyField = GetAttributeFields<PrimaryKey>(vertexType).FirstOrDefault();

                List<PropertyInfo> graphAttributeProperties = GetAttributeProperties<GraphAttribute>(vertexType);
                List<FieldInfo> graphAttributeFields = GetAttributeFields<GraphAttribute>(vertexType);

                if (primaryKeyProperty == null && primaryKeyField == null)
                    throw new Exception($"Implementation of IVertex must have Primary Key for type: {vertexType.Name}");

                string primaryIDName = primaryKeyProperty.Name ?? primaryKeyField.Name;
                PrimaryIDTypes primaryIDType = _valueConverter
                    .ConvertVertexPrimaryIDTypes(primaryKeyProperty.PropertyType ?? primaryKeyField.FieldType);

                TigerSchemaVertex vertex = new TigerSchemaVertex(vertexName.GetName(), primaryIDName, primaryIDType);

                List<TigerSchemaAttribute> attributes = GetTigergraphAttributes(graphAttributeProperties, graphAttributeFields);

                foreach (var attribute in attributes)
                    vertex.AddAttribute(attribute);

                graph.AddVertex(vertex);
            }

            return graph; 
        }

        private TigerSchemaGraph AddEdges(TigerSchemaGraph graph, List<Assembly> assemblies, NamespaceAnalyzer analyzer)
        {
            List<Type> edgeTypes = analyzer
                .GetCallerNamespaceTypesImplementingInterface<IEdge>()
                .Distinct()
                .ToList();

            foreach (Type edgeType in edgeTypes)
            {
                EdgeName edgeName = (EdgeName)Attribute.GetCustomAttribute(edgeType, typeof(EdgeName));

                List<PropertyInfo> graphAttributeProperties = GetAttributeProperties<GraphAttribute>(edgeType);
                List<FieldInfo> graphAttributeFields = GetAttributeFields<GraphAttribute>(edgeType);

                TigerSchemaEdge edge = new TigerSchemaEdge(edgeName.GetName());

                List<TigerSchemaAttribute> attributes = GetTigergraphAttributes(graphAttributeProperties, graphAttributeFields);

                foreach (var attribute in attributes)
                    edge.AddAttribute(attribute); 

                graph.AddEdge(edge);
            }

            return graph;
        }

        private List<TigerSchemaAttribute> GetTigergraphAttributes(List<PropertyInfo> attributeProperties, List<FieldInfo> attributeFields)
        {
            List<TigerSchemaAttribute> Attributes = new List<TigerSchemaAttribute>();

            foreach (var property in attributeProperties)
            {
                Type type = property.PropertyType;
                string name = property.Name;

                TigerSchemaAttribute attribute = CreateAttribute(type, name);
                Attributes.Add(attribute); 
            }

            foreach (var field in attributeFields)
            {
                Type type = field.FieldType;
                string name = field.Name;

                TigerSchemaAttribute attribute = CreateAttribute(type, name);
                Attributes.Add(attribute);
            }

            return Attributes;

        }

        private TigerSchemaAttribute CreateAttribute(Type type, string name)
        {
            AttributeTypes attributeType = _valueConverter.ConvertAttribute(type);

            object defaultValue = null;

            if (type.IsValueType)
                defaultValue = Activator.CreateInstance(type);

            TigerSchemaAttribute attribute = new TigerSchemaAttribute(name, attributeType, defaultValue);
            return attribute;
        }


        private List<PropertyInfo> GetAttributeProperties<AttributeType>(Type vertexType)
        {
            return vertexType.GetProperties()
                .Where(prop => prop.GetCustomAttributes(typeof(AttributeType), false).Any())
                .ToList();
        }

        private List<FieldInfo> GetAttributeFields<AttributeType>(Type vertexType)
        {
            return vertexType.GetFields()
                .Where(field => field.GetCustomAttributes(typeof(AttributeType), false).Any())
                .ToList();
        }

        private TigerSchemaGraph AddSourceTargetPairsForDataInsertions(IEnumerable<InvocationExpressionSyntax> dataInserts, NamespaceAnalyzer analyzer, TigerSchemaGraph graph)
        {
            foreach (var dataInsert in dataInserts)
            {
                List<Type> types = analyzer.GetInvocationMethodParameterTypes(dataInsert).ToList();
                var arguments = analyzer.GetInvocationMethodArguments(dataInsert);

                Type fromVertexType = types[0];
                Type edge = types[1];
                Type toVertexType = types[2];
                VertexName fromVertexName = (VertexName)Attribute.GetCustomAttribute(fromVertexType, fromVertexType.GetType());
                VertexName toVertexName = (VertexName)Attribute.GetCustomAttribute(toVertexType, toVertexType.GetType());

                TigerSchemaEdge schemaEdge;

                if (edge.IsPrimitive)
                    schemaEdge = new TigerSchemaEdge(arguments[1].ToString());
                else
                {
                    EdgeName edgeName = (EdgeName)Attribute.GetCustomAttribute(edge, edge.GetType());
                    schemaEdge = graph.Edges[edgeName.GetName()];
                }

                var fromVertex = graph.Vertices[fromVertexName.GetName()];
                var toVertex = graph.Vertices[toVertexName.GetName()];
                schemaEdge.AddSourceTargetPair(fromVertex, toVertex);

                graph.UpdateEdge(schemaEdge);
            }

            return graph;
        }

        private TigerSchemaGraph AddSourceTargetPairsForTrackingEvents(IEnumerable<InvocationExpressionSyntax> trackingEvents, NamespaceAnalyzer analyzer, TigerSchemaGraph graph)
        {
            if (trackingEvents.Any())
                HasEventTracking = true;

            foreach (var trackingEvent in trackingEvents)
            {
                List<Type> types = analyzer.GetInvocationMethodParameterTypes(trackingEvent).ToList();
                var arguments = analyzer.GetInvocationMethodArguments(trackingEvent);

                Type fromVertexType = types[0];
                VertexName fromVertexName = (VertexName)Attribute.GetCustomAttribute(fromVertexType, fromVertexType.GetType());

                var fromVertex = graph.Vertices[fromVertexName.GetName()];

                graph = AddEventSchema(fromVertex, graph);
            }

            return graph;
        }

        private TigerSchemaGraph AddSourceTargetPairsForErrors(IEnumerable<InvocationExpressionSyntax> errorEvents, NamespaceAnalyzer analyzer, TigerSchemaGraph graph)
        {
            if (errorEvents.Any())
                HasErrorHandling = true;

            foreach (var errorEvent in errorEvents)
            {
                List<Type> types = analyzer.GetInvocationMethodParameterTypes(errorEvent).ToList();
                var arguments = analyzer.GetInvocationMethodArguments(errorEvent);

                Type fromVertexType = types[0];
                VertexName fromVertexName = (VertexName)Attribute.GetCustomAttribute(fromVertexType, fromVertexType.GetType());

                var fromVertex = graph.Vertices[fromVertexName.GetName()];

                graph = AddErrorSchema(fromVertex, graph);
            }

            return graph;
        }

        private TigerSchemaGraph AddErrorSchema(TigerSchemaVertex sourceVertex, TigerSchemaGraph graph)
        {
            string sourceVertexName = sourceVertex.Name; 

            TigerSchemaVertex vertex = new TigerSchemaVertex("ErrorEvent", "EventID", PrimaryIDTypes.STRING);
            vertex.Attributes.Add(new TigerSchemaAttribute("Message", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("Source", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("InnerException", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("Timestamp", AttributeTypes.DATETIME));
            vertex.Attributes.Add(new TigerSchemaAttribute("StackTrace", AttributeTypes.STRING));

            TigerSchemaEdge edge = new TigerSchemaEdge($"{sourceVertexName}_Threw_Exception", isDirected: true, reverseEdge: $"Thrown_By_{sourceVertexName}");

            edge.AddSourceTargetPair(sourceVertex, vertex); 

            graph.AddVertex(vertex);
            graph.AddEdge(edge);

            return graph;
        }

        private TigerSchemaGraph AddEventSchema(TigerSchemaVertex sourceVertex, TigerSchemaGraph graph)
        {
            string sourceVertexName = sourceVertex.Name;

            TigerSchemaVertex vertex = new TigerSchemaVertex("Event", "EventID", PrimaryIDTypes.STRING);
            vertex.Attributes.Add(new TigerSchemaAttribute("EventDescription", AttributeTypes.STRING));

            TigerSchemaEdge edge = new TigerSchemaEdge($"{sourceVertexName}_Has_Event", isDirected: true, reverseEdge: $"Invoked_By_{sourceVertexName}");

            edge.AddSourceTargetPair(sourceVertex, vertex);

            graph.AddVertex(vertex);
            graph.AddEdge(edge);

            return graph;
        }

    }
}
