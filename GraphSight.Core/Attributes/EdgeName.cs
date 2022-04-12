using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GraphSight.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class EdgeName : Attribute
    {
        private string _name;

        public EdgeName(string name = null)
        {
            _name = name;
        }
    }
}
