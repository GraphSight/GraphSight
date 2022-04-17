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
    internal class NamespaceGraphSchemaBuilder
    {
        public bool HasErrorHandling { get; set; }
        public bool HasEventTracking { get; set; }

        private INamespaceAnalyzer _namespaceIterator;
        private IValueConverter _valueConverter;

        public NamespaceGraphSchemaBuilder() 
        {
            _namespaceIterator = new NamespaceAnalyzer();
            _valueConverter = new TigerValueConverter(); 

        }

        public void GenerateSchema(string graphName, List<Assembly> assemblies = null)
        {
            NamespaceAnalyzer analyzer = new NamespaceAnalyzer();

            var eventMethods = typeof(IEventTracker).GetMethods();

            TigerSchemaGraph graph = new TigerSchemaGraph(graphName);

            graph = AddVertices(graph, assemblies, analyzer);
            graph = AddEdges(graph, assemblies, analyzer);

            var invocations = analyzer.GetMethodInvocationsByAssembly(assemblies);

            var dataInserts = analyzer.GetMethodInvocationsByName(invocations, "TigerGraphDataInsert");
            var trackingEvents = analyzer.GetMethodInvocationsByName(invocations, "TigerGraphTrackEvent"); ;
            var errorEvents = analyzer.GetMethodInvocationsByName(invocations, "TigerGraphTrackError");

            if (trackingEvents.Any())
            {
                HasEventTracking = true;
                graph = AddEventSchema(graph); 
            }
            if (errorEvents.Any()) 
            {
                HasErrorHandling = true;
                graph = AddErrorVertex(graph); 
            }

            //From here on out: check parameters of each events/inserts, use them to build additional edges. 
            //Any GraphDataInsert with an event name as string will create a very generic edge. 


            //TODO (old instructions): 
            //First, call the namespaceiterator GetCallerNamespaceTypesImplementingInterface<T> to find ALL class implementing 
            //either IVertex or IEdge types.
            //Using these classes, create new graph nodes. Use namespace iterator GetCallerNamespaceTypesContainingAttribute
            //to fill in the attributes and names of each vertex and edge. 

            //For each event method from IEventTracker, we want to call namespaceIterator.GetCallerNamespaceMethodReferences().Count()  
            //on any items containing Event or ErrorEvent attributes. If the count is > 0, set HasErrorHandling/HasEventTracking, then use
            //the iterator to get the references, find the vertex types, and generate new graph nodes for containing data. 

            //we can also get method references like this example, in case we need this:
            //namespaceIterator.GetCallerNamespaceMethodReferences(typeof(GraphSightClient).GetMethod("TrackEvent", new Type[] { typeof(IVertex), typeof(string) }));
            //^^ Basically this would get an instance of that call with TWO DEFINED PARAMETERS.

        }

        private TigerSchemaGraph AddVertices(TigerSchemaGraph graph, List<Assembly> assemblies, NamespaceAnalyzer analyzer)
        {
            List<TigerSchemaVertex> vertices = new List<TigerSchemaVertex>(); 

            List<Type> vertexTypes = analyzer
                .GetCallerNamespaceTypesImplementingInterface<IVertex>(assemblies)
                .Distinct()
                .ToList();

            foreach (Type vertexType in vertexTypes)
            {
                VertexName vertexName = (VertexName)Attribute.GetCustomAttribute(vertexType, typeof(VertexName));

                PropertyInfo primaryKeyProperty = GetAttributeProperties(vertexType, typeof(PrimaryKey)).FirstOrDefault();
                FieldInfo primaryKeyField = GetAttributeFields(vertexType, typeof(PrimaryKey)).FirstOrDefault();

                List<PropertyInfo> graphAttributeProperties = GetAttributeProperties(vertexType, typeof(GraphAttribute));
                List<FieldInfo> graphAttributeFields = GetAttributeFields(vertexType, typeof(GraphAttribute));

                if (primaryKeyProperty == null && primaryKeyField == null)
                    throw new Exception($"Implementation of IVertex must have Primary Key for type: {vertexType.Name}");

                string primaryIDName = primaryKeyProperty.Name ?? primaryKeyField.Name;
                PrimaryIDTypes primaryIDType = _valueConverter
                    .ConvertVertexPrimaryIDTypes(primaryKeyProperty.GetType() ?? primaryKeyField.GetType());

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
                .GetCallerNamespaceTypesImplementingInterface<IEdge>(assemblies)
                .Distinct()
                .ToList();

            foreach (Type edgeType in edgeTypes)
            {
                EdgeName edgeName = (EdgeName)Attribute.GetCustomAttribute(edgeType, typeof(EdgeName));

                List<PropertyInfo> graphAttributeProperties = GetAttributeProperties(edgeType, typeof(GraphAttribute));
                List<FieldInfo> graphAttributeFields = GetAttributeFields(edgeType, typeof(GraphAttribute));

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


        private List<PropertyInfo> GetAttributeProperties<AttributeType>(Type T, AttributeType attribute)
        {
            return T.GetProperties()
                .Where(prop => prop.IsDefined(attribute.GetType(), false))
                .ToList();
        }

        private List<FieldInfo> GetAttributeFields<AttributeType>(Type T, AttributeType attribute)
        {
            return T.GetFields()
                .Where(prop => prop.IsDefined(attribute.GetType(), false))
                .ToList();
        }

        private TigerSchemaGraph AddErrorVertex(TigerSchemaGraph graph)
        {
            TigerSchemaVertex vertex = new TigerSchemaVertex("ErrorEvent", "EventID", PrimaryIDTypes.STRING);
            vertex.Attributes.Add(new TigerSchemaAttribute("Message", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("Source", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("InnerException", AttributeTypes.STRING));
            vertex.Attributes.Add(new TigerSchemaAttribute("Timestamp", AttributeTypes.DATETIME));
            vertex.Attributes.Add(new TigerSchemaAttribute("StackTrace", AttributeTypes.STRING));

            TigerSchemaEdge edge = new TigerSchemaEdge("ThrewException", true);

            graph.AddVertex(vertex);
            graph.AddEdge(edge);

            return graph;
        }

        private TigerSchemaGraph AddEventSchema(TigerSchemaGraph graph)
        {
            TigerSchemaVertex vertex = new TigerSchemaVertex("Event", "EventID", PrimaryIDTypes.STRING);
            vertex.Attributes.Add(new TigerSchemaAttribute("EventDescription", AttributeTypes.STRING));

            TigerSchemaEdge edge = new TigerSchemaEdge("HasEvent", true);

            graph.AddVertex(vertex);
            graph.AddEdge(edge);

            return graph;
        }
    }
}
