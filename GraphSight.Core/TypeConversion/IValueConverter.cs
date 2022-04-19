using GraphSight.Core.Enums.TigerGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Converters.TigerGraph
{
    interface IValueConverter
    {
        QueryParams ConvertQueryParameter(Type valueType);
        QueryParams ConvertQueryParameter(object value);
        AttributeTypes ConvertAttribute(Type valueType);
        AttributeTypes ConvertAttribute(object value);
        PrimaryIDTypes ConvertVertexPrimaryIDTypes(Type valueType);
        PrimaryIDTypes ConvertVertexPrimaryIDTypes(object value);
    }
}
