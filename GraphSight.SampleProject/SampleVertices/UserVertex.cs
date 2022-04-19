using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject.Vertices
{
    [VertexName("User")]
    class UserVertex : IVertex
    {
        [PrimaryKey]
        public string UserID { get; set; }

        [GraphAttribute]
        public string Name { get; set; }

    }
}
