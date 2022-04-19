using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    internal static class ReflectionUtils
    {
        public static List<PropertyInfo> GetAttributeProperties<AttributeType>(Type vertexType)
        {
            return vertexType.GetProperties()
                .Where(prop => prop.GetCustomAttributes(typeof(AttributeType), false).Any())
                .ToList();
        }

        public static List<FieldInfo> GetAttributeFields<AttributeType>(Type vertexType)
        {
            return vertexType.GetFields()
                .Where(field => field.GetCustomAttributes(typeof(AttributeType), false).Any())
                .ToList();
        }
    }
}
