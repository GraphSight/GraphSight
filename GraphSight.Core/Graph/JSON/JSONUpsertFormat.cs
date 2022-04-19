using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    internal class JsonUpsertFormat
    {
        [JsonProperty("@vertices")]
        Dictionary<string, JsonVertexType> vertices { get; set; }
        [JsonProperty("@edges")]
        Dictionary<string, JsonSourceVertexType> edges { get; set; }
    }

    internal class JsonVertexType
    {
        public Dictionary<string, JsonVertexID> vertexIDs { get; set; }
        public JsonVertexType(Dictionary<string, JsonVertexID> vertexIDs)
        {
            this.vertexIDs = vertexIDs;
        }

    }

    internal class JsonVertexID
    {
        public Dictionary<string, JsonAttribute> jsonAttributes {get;set;}

        public JsonVertexID(Dictionary<string, JsonAttribute> jsonAttributes)
        {
            this.jsonAttributes = jsonAttributes;
        }
    }

    internal class JsonAttribute
    {
        public object value { get; set; }
        //public int op { get; set; } //Todo: Implement

        public JsonAttribute(object value/*, int op = null*/)
        {
            this.value = value;
            //this.op = op;
        }
    }

    internal class JsonSourceVertexType
    {
        public Dictionary<string, JsonEdgeType> sourceVertices { get; set; }

        public JsonSourceVertexType(Dictionary<string, JsonEdgeType> sourceVertices)
        {
            this.sourceVertices = sourceVertices;
        }
    }

    internal class JsonEdgeType
    {
        public Dictionary<string, JsonTargetVertexType> targetVertices { get; set; }
        public JsonEdgeType(Dictionary<string, JsonTargetVertexType> targetVertices)
        {
            this.targetVertices = targetVertices;
        }

    }

    internal class JsonTargetVertexType
    {
        public Dictionary<string, JsonTargetVertexID> targetVertexIDs { get; set; }
        public JsonTargetVertexType(Dictionary<string, JsonTargetVertexID> targetVertexIDs)
        {
            this.targetVertexIDs = targetVertexIDs;
        }

    }

    internal class JsonTargetVertexID
    {
        public Dictionary<string, JsonAttribute> jsonAttributes { get; set; }

        public JsonTargetVertexID(Dictionary<string, JsonAttribute> jsonAttributes)
        {
            this.jsonAttributes = jsonAttributes;
        }
    }
}

