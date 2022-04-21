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
            //Todo: This code will likely require some cleanup work. Dictionary hell. 
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

            string sourceKey = GetValueOfPrimaryKey(source, sourcePrimaryKeyProperty, sourcePrimaryKeyField);
            string targetKey = GetValueOfPrimaryKey(target, targetPrimaryKeyProperty, targetPrimaryKeyField);

            //**Get vertices**
            fmt.vertices = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();

            //source
            var sourceVertexIds = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            var sourceVertexAttributes = GetJsonAttributes(source, sourceProperties, sourceFields);
            sourceVertexIds.Add(sourceKey, sourceVertexAttributes);
            fmt.vertices.Add(sourceName.GetName(), sourceVertexIds);

            //target
            var targetVertexIds = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            var targetVertexAttributes = GetJsonAttributes(target, targetProperties, targetFields);
            targetVertexIds.Add(targetKey, targetVertexAttributes);
            fmt.vertices.Add(targetName.GetName(), targetVertexIds);


            //**Get edges**
            fmt.edges = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>>>();

            var edgeSourceTypes = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>>();
            var edgeSourceIDs = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>();
            var edgeTypes = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>();
            var edgeAttributes = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            edgeAttributes.Add(targetKey, GetJsonAttributes(edge, edgeProperties, edgeFields));
            edgeTypes.Add(targetName.GetName(), edgeAttributes);
            edgeSourceIDs.Add(edgeName.GetName(), edgeTypes);
            edgeSourceTypes.Add(sourceKey, edgeSourceIDs);


            fmt.edges.Add(sourceName.GetName(), edgeSourceTypes);


            return fmt;
        }

        private Dictionary<string, Dictionary<string, string>> GetJsonAttributes(object data, List<PropertyInfo> properties, List<FieldInfo> fields)
        {
            Dictionary<string, Dictionary<string, string>> attrs = new Dictionary<string, Dictionary<string, string>>();

            foreach (var prop in properties)
            {
                var val = prop.GetValue(data);
                var name = prop.Name;
                attrs.Add(name, new Dictionary<string, string>() { { "value", val.ToString() }/*,{"op", ""} */ }); //TODO Implement op code
            }

            foreach (var field in fields)
            {
                var val = field.GetValue(data);
                var name = field.Name;
                attrs.Add(name, new Dictionary<string, string>() { { "value", val.ToString() }/*,{"op", ""} */ }); //TODO Implement op code
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
