using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class SequenceEvent : Attribute
    {
    }
}
