using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GraphSight.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class VertexName : Attribute
    {
        private string _name;

        public VertexName(string name = null)
        {
            _name = name;
        }
    }
}
