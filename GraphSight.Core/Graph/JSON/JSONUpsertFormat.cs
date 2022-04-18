using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    //Use this to dynamically set the edge/vertex names: https://stackoverflow.com/questions/37917164/newtonsoft-json-dynamic-property-name
    internal class JsonUpsertFormat
    {
        [JsonProperty("@vertices")]
        List<JsonVertexType> vertices { get; set; }
        [JsonProperty("@edges")]
        List<JsonEdgeType> edges { get; set; }
    }

    internal class JsonVertexType
    {
        List<JsonVertexID> vertexIDs { get; set; }
    }

    internal class JsonVertexID
    {
        List<JsonAttribute> jsonAttributes {get;set;}
    }

    internal class JsonAttribute
    {
        object value { get; set; }
        int op { get; set; }
    }

    internal class JsonEdgeType
    {
        //TODO
    }


}

