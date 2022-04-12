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
        private Type[] _namespaceTypes; 
        public NamespaceIterator() 
        {
            //Declare at construction to aid performance
            _namespaceTypes = GetNamespaceTypes(); 
        }

        public IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute)
        {
            Type[] types = GetNamespaceTypes();

            IEnumerable<Type> attributeTypes = _namespaceTypes
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));

            return attributeTypes;
        }

        public IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(T interfaceType)
        {
            Type[] types = GetNamespaceTypes();

            IEnumerable<Type> interfacedImplementers = _namespaceTypes
                .Where(type => type.GetInterfaces().Contains(typeof(T)));

            return interfacedImplementers;
        }

        private static Type[] GetNamespaceTypes()
        {
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            Type[] types = methodInfo.Module.Assembly.GetTypes();
            return types;
        }
    }
}
