using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GraphSight.Core
{
    public interface INamespaceAnalyzer 
    {
        IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute, List<Assembly> assemblies);
        IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(List<Assembly> assemblies);
        IEnumerable<MethodInfo> GetCallerNamespaceMethodInfos(MethodInfo methodInfo, List<Assembly> assemblies);
        IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByAssembly(List<Assembly> assemblies);
        IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByName(IEnumerable<InvocationExpressionSyntax> methodInvocations, string methodName);
    }

    public class NamespaceAnalyzer : INamespaceAnalyzer
    {
        public NamespaceAnalyzer() 
        {
        }

        public IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute, List<Assembly> assemblies = null)
        {
            IEnumerable<Type> assemblyTypes = (assemblies == null) ? 
                GetAssemblyTypes(GetExternalAssembly()) : GetAssemblyTypes(assemblies);

            return assemblyTypes
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));
        }

        public IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>(List<Assembly> assemblies = null)
        {
            IEnumerable<Type> assemblyTypes = (assemblies == null) ? 
                GetAssemblyTypes(GetExternalAssembly()) : GetAssemblyTypes(assemblies);

            return assemblyTypes
                .Where(type => type.GetInterfaces().Contains(typeof(T)));
        }

        public IEnumerable<MethodInfo> GetCallerNamespaceMethodInfos(MethodInfo methodInfo, List<Assembly> assemblies = null)
        {
            IEnumerable<MethodInfo> assemblyMethods = (assemblies == null) ?
                GetAssemblyMethods(GetExternalAssembly()) : GetAssemblyMethods(assemblies);

            return assemblyMethods
                .Where(m => m.MetadataToken == methodInfo.MetadataToken);
        }

        public IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByAssembly(List<Assembly> assemblies = null) 
        {
            if (assemblies == null)
                assemblies = new List<Assembly>() { GetExternalAssembly() };

            List<InvocationExpressionSyntax> methodList = new List<InvocationExpressionSyntax>();

            foreach (var assembly in assemblies) 
            {
                string projectPath = GetAssemblyProjectPath(assembly);

                var workspace = MSBuildWorkspace.Create();
                var project = workspace.OpenProjectAsync(projectPath).Result;
                project = GetProjectWithSourceFiles(project);

                var compilation = project.GetCompilationAsync().Result;

                var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();

                if (syntaxTree == null)
                    throw new Exception("Could not locate syntax tree for given assembly.");

                SemanticModel model = compilation.GetSemanticModel(syntaxTree);

                var invokedMethods = syntaxTree
                    .GetRootAsync().Result
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .ToList();

                //Test: 
                //var test = GetMethodInvocationsByName(invokedMethods, "testMethod").First();
                //var test2 = GetInvocationMethodParameterTypes(assembly, model, test); 

                methodList.AddRange(invokedMethods);
            }

            return methodList; 
        }

        public IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByName(IEnumerable<InvocationExpressionSyntax> methodInvocations, string methodName)
        {
            return methodInvocations.Where(s => s.Expression.GetText().ToString().Contains(methodName));
        }

        public List<Type> GetInvocationMethodParameterTypes(Assembly assembly, SemanticModel model, InvocationExpressionSyntax invocationExpression)
        {
            var methodSymbol = model.GetSymbolInfo(invocationExpression).Symbol;
            var declaringTypeName = string.Format(
                "{0}.{1}",
                methodSymbol.ContainingType.ContainingAssembly.Name,
                methodSymbol.ContainingType.Name
            );
            var methodName = methodSymbol.Name;

            List<Type> methodArgumentTypes = ((IMethodSymbol)methodSymbol)
                .Parameters
                .Select(p => assembly.GetType($"{p.Type.ContainingNamespace}.{ p.Type.Name}"))
                .ToList();

            return methodArgumentTypes;

        }

        private string GetAssemblyProjectPath(Assembly assembly)
        {
            string assemblyPath = Directory
                .GetParent(assembly.Location)
                .Parent
                .Parent
                .FullName;

            string assemblyName = GetExternalAssembly().GetName().Name;
            string projectPath = $@"{assemblyPath}\{assemblyName}.csproj";

            return projectPath;
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

        private Project GetProjectWithSourceFiles(Project project)
        {
            string projectDirectory = Directory.GetParent(project.FilePath).FullName;
            var files = GetAllSourceFiles(projectDirectory);

            foreach(var file in files) 
                project = project.AddDocument(file, File.ReadAllText(file)).Project;

            return project;
        }

        private IEnumerable<string> GetAllSourceFiles(string directoryPath)
        {
            var res = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

            return res;
        }

        /// <summary>
        /// Get the first assembly in the stack trace that is external to the calling assembly. 
        /// </summary>
        /// <returns></returns>
        private Assembly GetExternalAssembly() 
        {
            Assembly externalAssembly = new StackTrace()
                .GetFrames()
                .Where(s => s.GetMethod().Module.Assembly != Assembly.GetExecutingAssembly())
                .Select(s => s.GetMethod().Module.Assembly)
                .FirstOrDefault();

            return externalAssembly ?? Assembly.GetExecutingAssembly(); 
        }

        private IEnumerable<Type> GetAssemblyTypes(Assembly assembly) => assembly.GetTypes();
        private IEnumerable<Type> GetAssemblyTypes(List<Assembly> assemblies) => assemblies.SelectMany(s => s.GetTypes());

        private IEnumerable<MethodInfo> GetAssemblyMethods(Assembly assembly) => GetAssemblyTypes(assembly).SelectMany(s => s.GetMethods());
        private IEnumerable<MethodInfo> GetAssemblyMethods(List<Assembly> assemblies) => GetAssemblyTypes(assemblies).SelectMany(s => s.GetMethods());

        private MethodInfo GetExpressionMethod<T>(Expression<Func<T>> method)
        {
            MethodCallExpression mce = method.Body as MethodCallExpression;
            return mce.Method; 
        }
    }
}
