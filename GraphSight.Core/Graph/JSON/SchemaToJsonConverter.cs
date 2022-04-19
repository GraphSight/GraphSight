using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    public class SchemaToJsonConverter
    {
        INamespaceAnalyzer _namespaceAnalyzer;
        public SchemaToJsonConverter()
        {
            _namespaceAnalyzer = new NamespaceAnalyzer(); 
        }

        public JsonUpsertFormat GetSourceDestinationFormat(IVertex source, IEdge edge, IVertex target)
        {
            //Todo: This code will likely require some cleanup work. 

            JsonUpsertFormat fmt = new JsonUpsertFormat();

            EdgeName edgeName = GetEdgeName(edge);
            VertexName sourceName = GetVertexName(source);
            VertexName targetName = GetVertexName(target);

            PropertyInfo sourcePrimaryKeyProperty = GetPrimaryKeyProperty(source);
            FieldInfo sourcePrimaryKeyField = GetPrimaryKeyField(source);

            PropertyInfo targetPrimaryKeyProperty = GetPrimaryKeyProperty(target);
            FieldInfo targetPrimaryKeyField = GetPrimaryKeyField(target);

            List<PropertyInfo> edgeProperties = GetEdgeProperties(edge);
            List<FieldInfo> edgeFields = GetEdgeFields(edge);
            List<PropertyInfo> sourceProperties = GetVertexProperties(source);
            List<FieldInfo> sourceFields = GetVertexFields(source);
            List<PropertyInfo> targetProperties = GetVertexProperties(target);
            List<FieldInfo> targetFields = GetVertexFields(target);


            //Set vertices data
            Dictionary<string, JsonVertexType> vertices = new Dictionary<string, JsonVertexType>();
            var sourceVertexIds = new Dictionary<string, JsonVertexID>();
            var targetVertexIds = new Dictionary<string, JsonVertexID>();

            var sourceVertexId = new JsonVertexID(GetJsonAttributes(source, sourceProperties, sourceFields));
            sourceVertexIds.Add(GetValueOfPrimaryKey(source, sourcePrimaryKeyProperty, sourcePrimaryKeyField), sourceVertexId);

            var targetVertexId = new JsonVertexID(GetJsonAttributes(target, targetProperties, targetFields));
            targetVertexIds.Add(GetValueOfPrimaryKey(target, targetPrimaryKeyProperty, targetPrimaryKeyField), targetVertexId);

            var sourceVertexType = new JsonVertexType(sourceVertexIds);
            var targetVertexType = new JsonVertexType(targetVertexIds);
            vertices.Add(sourceName.GetName(), sourceVertexType);
            vertices.Add(targetName.GetName(), targetVertexType);

            //Set edge data
            Dictionary<string, JsonSourceVertexType> edgeSourceTypes = new Dictionary<string, JsonSourceVertexType>();
            Dictionary<string, JsonEdgeType> edgeTypes = new Dictionary<string, JsonEdgeType>();
            var sourceVertices = new Dictionary<string, JsonEdgeType>();
            var targetVertexTypes = new Dictionary<string, JsonTargetVertexType>();
            var edgetargetVertexIds = new Dictionary<string, JsonTargetVertexID>();
            var edgeAttributes = GetJsonAttributes(edge, edgeProperties, edgeFields);

            var jsonTargetVertexId = new JsonTargetVertexID(edgeAttributes); 
            edgetargetVertexIds.Add(GetValueOfPrimaryKey(target, targetPrimaryKeyProperty, targetPrimaryKeyField), jsonTargetVertexId);
            var edgeTargetVertexType = new JsonTargetVertexType(edgetargetVertexIds);
            targetVertexTypes.Add(targetName.GetName(), edgeTargetVertexType);
            var edgeType = new JsonEdgeType(targetVertexTypes);
            sourceVertices.Add(GetValueOfPrimaryKey(source, sourcePrimaryKeyProperty, sourcePrimaryKeyField), edgeType);
            var edgeSourceVertexType = new JsonSourceVertexType(sourceVertices);
            edgeSourceTypes.Add(sourceName.GetName(), edgeSourceVertexType);

            fmt.vertices = vertices;
            fmt.edges = edgeSourceTypes;

            return fmt;
        }

        private Dictionary<string, JsonAttribute> GetJsonAttributes(object data, List<PropertyInfo> properties, List<FieldInfo> fields)
        {
            Dictionary<string, JsonAttribute> attrs = new Dictionary<string, JsonAttribute>();

            foreach (var prop in properties)
            {
                var val = prop.GetValue(data);
                var name = prop.Name;
                attrs.Add(name, new JsonAttribute(val));
            }

            foreach (var field in fields)
            {
                var val = field.GetValue(data);
                var name = field.Name;
                attrs.Add(name, new JsonAttribute(val));
            }

            return attrs; 
        }

        private string GetValueOfPrimaryKey(object vertex, PropertyInfo propertyInfo, FieldInfo fieldInfo)
        {
            if (propertyInfo != null)
                return propertyInfo.GetValue(vertex).ToString();
            else
                return fieldInfo.GetValue(vertex).ToString();
        }


        private static VertexName GetVertexName(IVertex vertex)
        {
            return (VertexName)Attribute.GetCustomAttribute(vertex.GetType(), typeof(VertexName));
        }

        private static EdgeName GetEdgeName(IEdge edge)
        {
            return (EdgeName)Attribute.GetCustomAttribute(edge.GetType(), typeof(EdgeName));
        }

        private static List<FieldInfo> GetVertexFields(IVertex source)
        {
            return ReflectionUtils.GetAttributeFields<GraphAttribute>(source.GetType());
        }

        private static List<PropertyInfo> GetVertexProperties(IVertex source)
        {
            return ReflectionUtils.GetAttributeProperties<GraphAttribute>(source.GetType());
        }

        private static List<FieldInfo> GetEdgeFields(IEdge edge)
        {
            return ReflectionUtils.GetAttributeFields<GraphAttribute>(edge.GetType());
        }

        private static List<PropertyInfo> GetEdgeProperties(IEdge edge)
        {
            return ReflectionUtils.GetAttributeProperties<GraphAttribute>(edge.GetType());
        }

        private static FieldInfo GetPrimaryKeyField(IVertex source)
        {
            return ReflectionUtils.GetAttributeFields<PrimaryKey>(source.GetType()).FirstOrDefault();
        }

        private static PropertyInfo GetPrimaryKeyProperty(IVertex source)
        {
            return ReflectionUtils.GetAttributeProperties<PrimaryKey>(source.GetType()).FirstOrDefault();
        }
    }
}
