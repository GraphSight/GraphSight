using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal interface ICredentials
    {
        string Username{ get; set; }
        string Password { get; set; }
        string URI { get; set; }
        string Secret { get; set; }
    }
}
