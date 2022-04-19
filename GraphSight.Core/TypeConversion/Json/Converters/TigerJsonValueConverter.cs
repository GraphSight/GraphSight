using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core.Converters.TigerGraph
{
    public class TigerJsonValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

        public override bool CanWrite { get { return true; } }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Console.WriteLine("test");
            var token = JToken.Load(reader);
            if (token.Type != JTokenType.Object)
                return token;
            var typeString = (string)token["$type"];
            if (typeString == null)
                return token;
            string typeName, assemblyName;
            SplitFullyQualifiedTypeName(typeString, out typeName, out assemblyName);
            var type = serializer.Binder.BindToType(assemblyName, typeName);
            if (type != null)
                return token.ToObject(type, serializer);
            return token;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TigerValueConverter converter = new TigerValueConverter();
            var item = converter.ConvertQueryParameter(value);

            writer.WriteValue(item);
            writer.Flush();
        }

        // Utilities taken from https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
        public static void SplitFullyQualifiedTypeName(string fullyQualifiedTypeName, out string typeName, out string assemblyName)
        {
            int? assemblyDelimiterIndex = GetAssemblyDelimiterIndex(fullyQualifiedTypeName);

            if (assemblyDelimiterIndex != null)
            {
                typeName = fullyQualifiedTypeName.Substring(0, assemblyDelimiterIndex.Value).Trim();
                assemblyName = fullyQualifiedTypeName.Substring(assemblyDelimiterIndex.Value + 1, fullyQualifiedTypeName.Length - assemblyDelimiterIndex.Value - 1).Trim();
            }
            else
            {
                typeName = fullyQualifiedTypeName;
                assemblyName = null;
            }
        }

        private static int? GetAssemblyDelimiterIndex(string fullyQualifiedTypeName)
        {
            int scope = 0;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char current = fullyQualifiedTypeName[i];
                switch (current)
                {
                    case '[':
                        scope++;
                        break;
                    case ']':
                        scope--;
                        break;
                    case ',':
                        if (scope == 0)
                            return i;
                        break;
                }
            }

            return null;
        }
    }
}
