using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject
{
    [EdgeName("Owns")]
    class UserHasAccountEdge : IEdge
    {
        [GraphAttribute]
        public DateTime DateCreated = DateTime.Today;
    }
}
