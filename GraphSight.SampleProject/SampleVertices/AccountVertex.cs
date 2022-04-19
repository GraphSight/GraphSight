using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject
{
    [VertexName("Account")]
    class AccountVertex : IVertex
    {
        [PrimaryKey]
        public string AccountID { get; set; }
        [GraphAttribute]
        public string Status { get; set; }

    }
}
