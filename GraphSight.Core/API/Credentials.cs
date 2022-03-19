using GraphSight.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core
{
    internal class Credentials : ICredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string URI { get; set; }
        public string Secret { get; set; }
    }
}
