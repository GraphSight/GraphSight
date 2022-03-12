using GraphSight.Core.Enums.TigerGraph;

namespace GraphSight.Core.Graph
{
    internal class VertexAttribute
    {
        public string Name { get; set; }
        public AttributeTypes Type { get; set; }
        public object Value { get; set; }
        public object DefaultValue { get; set; }
        public bool IsIndex { get; set; }

        public VertexAttribute()
        {
        }

        public VertexAttribute(string name,
            AttributeTypes type,
            object value,
            object defaultValue,
            bool isIndex)
        {
            Name = name;
            Type = type;
            Value = value;
            DefaultValue = defaultValue;
            IsIndex = isIndex;
        }
    }
}
