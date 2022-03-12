using GraphSight.Core.Enums.TigerGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Converters.TigerGraph
{
    internal class TigerConverter : IConverter
    {

        internal QueryParams ConvertQueryParameter(object value) {
            Type valueType = value.GetType();

            if(valueType == typeof(int)) return QueryParams.INT;
            if (valueType == typeof(uint)) return QueryParams.UINT;
            if (valueType == typeof(float)) return QueryParams.FLOAT;
            if (valueType == typeof(double)) return QueryParams.DOUBLE;
            if (valueType == typeof(char)) return QueryParams.STRING;
            if (valueType == typeof(string)) return QueryParams.STRING;
            if (valueType == typeof(bool)) return QueryParams.BOOL;
            if (valueType == typeof(DateTime)) return QueryParams.DATETIME;

            throw new ArgumentException($"Object type {valueType.Name} could not be converted into a TigerGraph Query Parameter.");
        }

        internal VertexAttributes ConvertVertexAttribute(object value)
        {
            Type valueType = value.GetType();

            if (valueType == typeof(int)) return VertexAttributes.INT;
            if (valueType == typeof(uint)) return VertexAttributes.UINT;
            if (valueType == typeof(bool)) return VertexAttributes.BOOL;
            if (valueType == typeof(float)) return VertexAttributes.FLOAT;
            if (valueType == typeof(double)) return VertexAttributes.DOUBLE;
            if (valueType == typeof(char)) return VertexAttributes.STRING;
            if (valueType == typeof(string)) return VertexAttributes.STRING;
            if (valueType == typeof(DateTime)) return VertexAttributes.DATETIME;

            if (valueType == typeof(List<>)) return VertexAttributes.LIST;
            if (valueType == typeof(LinkedList<>)) return VertexAttributes.LIST;
            if (valueType == typeof(Stack<>)) return VertexAttributes.LIST;
            if (valueType == typeof(Queue<>)) return VertexAttributes.LIST;
            if (valueType.IsArray) return VertexAttributes.LIST;

            if (valueType == typeof(Dictionary<,>)) return VertexAttributes.MAP;

            if (valueType == typeof(Tuple)) return VertexAttributes.UDT;

            throw new ArgumentException($"Object type {valueType.Name} could not be converted into a TigerGraph Vertex Attribute.");
        }

        internal VertexPrimaryIDTypes ConvertVertexPrimaryIDTypes(object value)
        {
            Type valueType = value.GetType();

            if (valueType == typeof(char)) return VertexPrimaryIDTypes.STRING;
            if (valueType == typeof(string)) return VertexPrimaryIDTypes.STRING;
            if (valueType == typeof(int)) return VertexPrimaryIDTypes.INT;
            if (valueType == typeof(uint)) return VertexPrimaryIDTypes.UINT;
            if (valueType == typeof(float)) return VertexPrimaryIDTypes.FLOAT;
            if (valueType == typeof(double)) return VertexPrimaryIDTypes.DOUBLE;
            if (valueType == typeof(DateTime)) return VertexPrimaryIDTypes.DATETIME;

            throw new ArgumentException($"Object type {valueType.Name} could not be converted into a TigerGraph Vertex Primary ID type.");
        }
    }
}
