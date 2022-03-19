﻿using System.Collections.Generic;
using System.Linq;

namespace GraphSight.Core.Extensions
{
    public static class Extensions
    {
        public static string FromDictionaryToJson(this Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, kvp.Value));
            return string.Concat("{", string.Join(",", kvs), "}");
        }

        public static Dictionary<string, string> FromJsonToDictionary(this string json)
        {
            string[] keyValueArray = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return keyValueArray.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }

        public static string WithoutTrailingSlash(this string address)
        {
            return address.TrimEnd('/');
        }
    }
}
