using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    internal class SchemaToJsonConverter
    {
        INamespaceAnalyzer _namespaceAnalyzer;
        public SchemaToJsonConverter()
        {
            _namespaceAnalyzer = new NamespaceAnalyzer(); 
        }

        public JsonUpsertFormat GetSourceDestinationFormat(IVertex source, IEdge edge, IVertex target)
        {
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

            //Todo: finish here

            var sourceVertexType = new JsonVertexType(sourceVertexIds);
            var targetVertexType = new JsonVertexType(targetVertexIds);
            vertices.Add(sourceName.GetName(), sourceVertexType);
            vertices.Add(targetName.GetName(), targetVertexType);

            //Set edge data


            return fmt;
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
