using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    internal interface INamespaceIterator 
    {
        IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute);
        IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(T interfaceType);
    }

    internal class NamespaceIterator
    {
        public IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute) 
        {
            //Get available namespace types
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            Type[] types = methodInfo.Module.Assembly.GetTypes();

            IEnumerable<Type> attributeTypes = types
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));

            return attributeTypes; 
        }

        public IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(T interfaceType)
        {

            //Get available namespace types
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            Type[] types = methodInfo.Module.Assembly.GetTypes();

            IEnumerable<Type> interfacedImplementers = types
                .Where(type => type.GetInterfaces().Contains(typeof(T)));

            return interfacedImplementers;
        }
    }
}
