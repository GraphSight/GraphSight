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

        public IEnumerable<MethodInfo> GetCallerNamespaceMethodInfos(MethodInfo methodInfo, List<Assembly> assemblies = null)
        {
            IEnumerable<MethodInfo> assemblyMethods = (assemblies == null) ?
                GetAssemblyMethods(GetExternalAssembly()) : GetAssemblyMethods(assemblies);

            return assemblyMethods
                .Where(m => m.MetadataToken == methodInfo.MetadataToken);
        }

        public int GetCallerNamespaceMethodCount(MethodInfo methodInfo, List<Assembly> assemblies = null)
        {
            return GetCallerNamespaceMethodInfos(methodInfo, assemblies).Count(); 
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

        public IEnumerable<ReferencedSymbol> GetMethodReferences()
        {
            string assemblyPath = Directory.GetParent(GetExternalAssembly().Location).Parent.Parent.FullName;
            string assemblyName = GetExternalAssembly().GetName().Name;
            string projectPath = $@"{assemblyPath}\{assemblyName}.csproj";

            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(projectPath).Result;
            project = GetProjectWithSourceFiles(project);

            var compilation = project.GetCompilationAsync().Result;

            var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();

            if (syntaxTree == null)
                throw new Exception("Could not locate syntax tree for given assembly.");

            //for reference
            //var rootSyntaxNode = syntaxTree.GetRootAsync().Result;
            //var firstLocalVariablesDeclaration = rootSyntaxNode.DescendantNodesAndSelf()
            //    .OfType<LocalDeclarationStatementSyntax>().First();
            //var firstVariable = firstLocalVariablesDeclaration.Declaration.Variables.First();
            //var variableInitializer = firstVariable.Initializer.Value.GetFirstToken().ValueText;


            SemanticModel model = compilation.GetSemanticModel(syntaxTree);
            
            var methodInvocation = syntaxTree.GetRootAsync().Result.DescendantNodes().OfType<InvocationExpressionSyntax>().Last();
            var methodSymbol = model.GetSymbolInfo(methodInvocation).Symbol;

            var references = SymbolFinder.FindReferencesAsync(methodSymbol, project.Solution).Result;

            return references;
        }

        private MethodInfo GetInvocationMethodInfo(SemanticModel model, InvocationExpressionSyntax invocationExpression) 
        {
            var methodSymbol = model.GetSymbolInfo(invocationExpression).Symbol;
            var declaringTypeName = string.Format(
                "{0}.{1}",
                methodSymbol.ContainingType.ContainingAssembly.Name,
                methodSymbol.ContainingType.Name
            );
            var methodName = methodSymbol.Name;
            var methodArgumentTypeNames = methodSymbol.Parameters.Select(
                p => p.Type.ContainingNamespace.Name + "." + p.Type.Name
            );
            var methodInfo = Type.GetType(declaringTypeName).GetMethod(
                methodName,
                methodArgumentTypeNames.Select(typeName => Type.GetType(typeName)).ToArray()
            );
            return methodInfo;
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
