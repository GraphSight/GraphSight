using GraphSight.Core.Enums.TigerGraph;

namespace GraphSight.Core.Graph
{
    internal class TigerEdgeAttribute
    {
        public string Name { get; set; }
        public AttributeTypes Type { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }

        public TigerEdgeAttribute()
        {
        }

        public TigerEdgeAttribute(string name,
            AttributeTypes type,
            object value,
            object defaultValue)
        {
            Name = name;
            Type = type;
            Value = value;
            DefaultValue = defaultValue;
        }
    }
}
