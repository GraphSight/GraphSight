using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal interface ICredentials
    {
        string Username{ get; set; }
        string Password { get; set; }
        string Domain { get; set; }
        string Secret { get; set; }
    }
}
