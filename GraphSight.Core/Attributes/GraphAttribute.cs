using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GraphSight.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class GraphAttribute : Attribute
    {
    }
}
