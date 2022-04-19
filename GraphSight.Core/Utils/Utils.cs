using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphSight.Core
{
    public static class Utils
    {
        #region JSONUtils
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
        #endregion

        #region StringUtils
        public static string WithoutTrailingSlash(this string address)
        {
            return address.TrimEnd('/');
        }
        #endregion

        #region TypeUtils
        public static bool isPrimitive(object o) => o.GetType().Namespace == "System";
        public static bool isPrimitive(Type t) => t.Namespace == "System";
        #endregion

    }
}
