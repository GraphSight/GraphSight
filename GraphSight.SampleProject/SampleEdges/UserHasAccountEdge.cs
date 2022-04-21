using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSight.SampleProject
{
    [EdgeName("User_Has_Account")]
    class UserHasAccountEdge : IEdge
    {
        [GraphAttribute]
        public DateTime DateCreated = DateTime.Today;
    }
}
