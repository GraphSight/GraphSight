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

        internal AttributeTypes ConvertVertexAttribute(object value)
        {
            Type valueType = value.GetType();

            if (valueType == typeof(int)) return AttributeTypes.INT;
            if (valueType == typeof(uint)) return AttributeTypes.UINT;
            if (valueType == typeof(bool)) return AttributeTypes.BOOL;
            if (valueType == typeof(float)) return AttributeTypes.FLOAT;
            if (valueType == typeof(double)) return AttributeTypes.DOUBLE;
            if (valueType == typeof(char)) return AttributeTypes.STRING;
            if (valueType == typeof(string)) return AttributeTypes.STRING;
            if (valueType == typeof(DateTime)) return AttributeTypes.DATETIME;

            if (valueType == typeof(List<>)) return AttributeTypes.LIST;
            if (valueType == typeof(LinkedList<>)) return AttributeTypes.LIST;
            if (valueType == typeof(Stack<>)) return AttributeTypes.LIST;
            if (valueType == typeof(Queue<>)) return AttributeTypes.LIST;
            if (valueType.IsArray) return AttributeTypes.LIST;

            if (valueType == typeof(Dictionary<,>)) return AttributeTypes.MAP;

            if (valueType == typeof(Tuple)) return AttributeTypes.UDT;

            throw new ArgumentException($"Object type {valueType.Name} could not be converted into a TigerGraph Vertex Attribute.");
        }

        internal PrimaryIDTypes ConvertVertexPrimaryIDTypes(object value)
        {
            Type valueType = value.GetType();

            if (valueType == typeof(char)) return PrimaryIDTypes.STRING;
            if (valueType == typeof(string)) return PrimaryIDTypes.STRING;
            if (valueType == typeof(int)) return PrimaryIDTypes.INT;
            if (valueType == typeof(uint)) return PrimaryIDTypes.UINT;
            if (valueType == typeof(float)) return PrimaryIDTypes.FLOAT;
            if (valueType == typeof(double)) return PrimaryIDTypes.DOUBLE;
            if (valueType == typeof(DateTime)) return PrimaryIDTypes.DATETIME;

            throw new ArgumentException($"Object type {valueType.Name} could not be converted into a TigerGraph Vertex Primary ID type.");
        }
    }
}
