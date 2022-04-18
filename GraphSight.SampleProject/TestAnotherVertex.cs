using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject
{
    [VertexName("Device")]
    class TestAnotherVertex : IVertex
    {
        [PrimaryKey]
        public string id { get; set; }
        [GraphAttribute]
        public string someData { get; set; }

    }
}
