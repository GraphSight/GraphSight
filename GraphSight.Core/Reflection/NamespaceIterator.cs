using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    public interface INamespaceIterator 
    {
        IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute);
        IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(T interfaceType);
    }

    public class NamespaceIterator
    {
        public NamespaceIterator() 
        {
        }

        public IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute, List<Assembly> assemblies = null)
        {
            IEnumerable<Type> assemblyTypes = (assemblies == null) ? 
                GetAssemblyTypes(GetExternalAssembly()) : GetAssemblyTypes(assemblies);

            return assemblyTypes
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));
        }

        public IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(T interfaceType, List<Assembly> assemblies = null)
        {
            IEnumerable<Type> assemblyTypes = (assemblies == null) ? 
                GetAssemblyTypes(GetExternalAssembly()) : GetAssemblyTypes(assemblies);

            return assemblyTypes
                .Where(type => type.GetInterfaces().Contains(typeof(T)));
        }

        public IEnumerable<MethodInfo> GetCallerNamespaceMethodReferences(MethodInfo methodInfo, List<Assembly> assemblies = null)
        {
            IEnumerable<MethodInfo> assemblyMethods = (assemblies == null) ?
                GetAssemblyMethods(GetExternalAssembly()) : GetAssemblyMethods(assemblies);

            return assemblyMethods
                .Where(m => m.MetadataToken == methodInfo.MetadataToken);
        }

        public int GetCallerNamespaceMethodCount(MethodInfo methodInfo, List<Assembly> assemblies = null)
        {
            return GetCallerNamespaceMethodReferences(methodInfo, assemblies).Count(); 
        }

        private static IEnumerable<Type> GetNamespaceTypes()
        {
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            IEnumerable<Type> types = methodInfo.Module.Assembly.GetTypes();
            return types;
        }

        private static IEnumerable<MethodInfo> GetNamespaceMethods() 
        {
            MethodBase methodInfo = new StackTrace().GetFrame(1).GetMethod();
            IEnumerable<MethodInfo> methods = methodInfo.Module.Assembly
                .GetTypes()
                .SelectMany(s => s.GetMethods());
            return methods;
        }

        /// <summary>
        /// Get the first assembly in the stack trace that is external to the calling assembly. 
        /// </summary>
        /// <returns></returns>
        private Assembly GetExternalAssembly() 
        {
            return new StackTrace()
                .GetFrames()
                .Where(s => s.GetMethod().Module.Assembly != Assembly.GetExecutingAssembly())
                .Select(s => s.GetMethod().Module.Assembly)
                .FirstOrDefault(); 
        }

        private IEnumerable<Type> GetAssemblyTypes(Assembly assembly) => assembly.GetTypes();
        private IEnumerable<Type> GetAssemblyTypes(List<Assembly> assemblies) => assemblies.SelectMany(s => s.GetTypes());

        private IEnumerable<MethodInfo> GetAssemblyMethods(Assembly assembly) => GetAssemblyTypes(assembly).SelectMany(s => s.GetMethods());
        private IEnumerable<MethodInfo> GetAssemblyMethods(List<Assembly> assemblies) => GetAssemblyTypes(assemblies).SelectMany(s => s.GetMethods());

        public MethodInfo GetExpressionMethod<T>(Expression<Func<T>> method)
        {
            MethodCallExpression mce = method.Body as MethodCallExpression;
            return mce.Method; 
        }
    }
}
