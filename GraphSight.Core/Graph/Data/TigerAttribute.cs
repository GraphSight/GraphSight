using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using Newtonsoft.Json;

namespace GraphSight.Core.Graph
{
    public class TigerAttribute
    {
        public string Name { get; set; }
        public AttributeTypes Type { get; set; }

        //[JsonConverter(typeof(TigerJsonValueConverter))]
        public object Value { get; set; }

        public TigerAttribute()
        {
        }

        public TigerAttribute(string name,
            AttributeTypes type,
            object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}
