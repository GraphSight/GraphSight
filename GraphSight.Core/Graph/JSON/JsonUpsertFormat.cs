using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    public class JsonUpsertFormat
    {
        [JsonProperty("vertices")]
        public Dictionary<string, JsonVertexType> vertices { get; set; }
        [JsonProperty("edges")]
        public Dictionary<string, JsonSourceVertexType> edges { get; set; }
    }

    public class JsonVertexType
    {
        public Dictionary<string, JsonVertexID> vertexIDs { get; set; }
        public JsonVertexType(Dictionary<string, JsonVertexID> vertexIDs)
        {
            this.vertexIDs = vertexIDs;
        }

    }

    public class JsonVertexID
    {
        public Dictionary<string, JsonAttribute> jsonAttributes {get;set;}

        public JsonVertexID(Dictionary<string, JsonAttribute> jsonAttributes)
        {
            this.jsonAttributes = jsonAttributes;
        }
    }

    public class JsonAttribute
    {
        public object value { get; set; }
        //public int op { get; set; } //Todo: Implement

        public JsonAttribute(object value/*, int op = null*/)
        {
            this.value = value;
            //this.op = op;
        }
    }

    public class JsonSourceVertexType
    {
        public Dictionary<string, JsonEdgeType> sourceVertices { get; set; }

        public JsonSourceVertexType(Dictionary<string, JsonEdgeType> sourceVertices)
        {
            this.sourceVertices = sourceVertices;
        }
    }

    public class JsonEdgeType
    {
        public Dictionary<string, JsonTargetVertexType> targetVertices { get; set; }
        public JsonEdgeType(Dictionary<string, JsonTargetVertexType> targetVertices)
        {
            this.targetVertices = targetVertices;
        }

    }

    public class JsonTargetVertexType
    {
        public Dictionary<string, JsonTargetVertexID> targetVertexIDs { get; set; }
        public JsonTargetVertexType(Dictionary<string, JsonTargetVertexID> targetVertexIDs)
        {
            this.targetVertexIDs = targetVertexIDs;
        }

    }

    public class JsonTargetVertexID
    {
        public Dictionary<string, JsonAttribute> jsonAttributes { get; set; }

        public JsonTargetVertexID(Dictionary<string, JsonAttribute> jsonAttributes)
        {
            this.jsonAttributes = jsonAttributes;
        }
    }
}

