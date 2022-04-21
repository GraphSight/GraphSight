using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph.JSON
{
    //This is disgusting, but it will work for now. 
    internal class JsonUpsertFormat
    {
        [JsonProperty("vertices")]
        public Dictionary<string,                       //VertexType
            Dictionary<string,                          //VertexID
                Dictionary<string,                      //Attribute
                    Dictionary<string, string>>>>
            vertices
        { get; set; }


        [JsonProperty("edges")]
        public Dictionary<string,                        //SourceVertexType
            Dictionary<string,                           //SourceVertexID
                Dictionary<string,                       //EdgeType
                    Dictionary<string,                   //TargetVertexType   
                        Dictionary<string,               //TargetVertexID
                            Dictionary<string,           //Attribute
                                Dictionary<string, string>>>>>>>
            edges
        { get; set; }
    }
}

