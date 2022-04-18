﻿using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject.Vertices
{
    [VertexName("User")]
    class TestVertex : IVertex
    {
        [PrimaryKey]
        public string primaryID { get; set; }

        [GraphAttribute]
        public string username { get; set; }

    }
}
