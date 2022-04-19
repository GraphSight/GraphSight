using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using Newtonsoft.Json;

namespace GraphSight.Core.Graph
{
    public class TigerSchemaAttribute
    {
        public string Name { get; set; }
        public AttributeTypes Type { get; set; }
        public object DefaultValue { get; set; }

        public TigerSchemaAttribute()
        {
        }

        public TigerSchemaAttribute(string name,
            AttributeTypes type,
            object defaultValue = null)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
    }
}
