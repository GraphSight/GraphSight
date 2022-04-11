using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    public class NamespaceIterator
    {
        public static IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute) 
        {
            //Get available namespace types
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            Type[] types = methodInfo.Module.Assembly.GetTypes();

            IEnumerable<Type> attributeTypes = types
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));

            return attributeTypes; 
        }
    }
}
