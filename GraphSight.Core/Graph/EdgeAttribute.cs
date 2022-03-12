using GraphSight.Core.Enums.TigerGraph;

namespace GraphSight.Core.Graph
{
    internal class EdgeAttribute
    {
        public string Name { get; set; }
        public AttributeTypes Type { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }

        public EdgeAttribute()
        {
        }

        public EdgeAttribute(string name,
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
