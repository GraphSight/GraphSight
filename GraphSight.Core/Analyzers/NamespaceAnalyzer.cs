using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute);
        IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>();
        IEnumerable<MethodInfo> GetCallerNamespaceMethodInfos(MethodInfo methodInfo);
        IEnumerable<InvocationExpressionSyntax> GetMethodInvocations();
        IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByName(string methodName);
        SeparatedSyntaxList<ArgumentSyntax> GetInvocationMethodArguments(InvocationExpressionSyntax invocationExpression);
        IEnumerable<Type> GetInvocationMethodParameterTypes(InvocationExpressionSyntax invocationExpression);
    }

    public class NamespaceAnalyzer : INamespaceAnalyzer
    {
        private List<Assembly> _assemblies;
        private Dictionary<Assembly, SemanticModel> _semanticModels;
        private Dictionary<Assembly, SyntaxTree> _syntaxTrees;
        private Dictionary<InvocationExpressionSyntax, SemanticModel> _methodInvocationModels;

        private SemanticModel _internalSemanticModel;


        public NamespaceAnalyzer(List<Assembly> assemblies = null)
        {
            _assemblies = assemblies ?? new List<Assembly>() { GetExternalAssembly() };

            _semanticModels = new Dictionary<Assembly, SemanticModel>();
            _syntaxTrees = new Dictionary<Assembly, SyntaxTree>();
            _methodInvocationModels = new Dictionary<InvocationExpressionSyntax, SemanticModel>();

            //Set internal model for locating internal method calls
            //GetSemanticModel(Assembly.GetExecutingAssembly(), out _, out _internalSemanticModel);
            var testingTree = CSharpSyntaxTree.ParseText(GetDocumentText());
            var compilationtwo = CSharpCompilation.Create("TestCompilation", new[] { testingTree });
            var modeltwo = compilationtwo.GetSemanticModel(testingTree);
            _internalSemanticModel = modeltwo;



            foreach (var assembly in _assemblies)
            {
                SyntaxTree syntaxTree;
                SemanticModel model;
                GetSemanticModel(assembly, out syntaxTree, out model);

                _semanticModels.Add(assembly, model);
                _syntaxTrees.Add(assembly, syntaxTree);

                var invokedMethods = syntaxTree
                    .GetRootAsync().Result
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .ToList();

                foreach (var invoked in invokedMethods)
                    _methodInvocationModels.Add(invoked, model);
            }

        }

        public IEnumerable<Type> GetCallerNamespaceTypesContainingAttribute(Attribute attribute)
        {
            IEnumerable<Type> assemblyTypes = GetAssemblyTypes(_assemblies);

            return assemblyTypes
                .Where(type => Attribute.IsDefined(type, attribute.GetType()));
        }

        public IEnumerable<Type> GetCallerNamespaceTypesImplementingInterface<T>()
        {
            IEnumerable<Type> assemblyTypes = GetAssemblyTypes(_assemblies);

            return assemblyTypes
                .Where(type => type.GetInterfaces().Contains(typeof(T)));
        }

        public IEnumerable<MethodInfo> GetCallerNamespaceMethodInfos(MethodInfo methodInfo)
        {
            IEnumerable<MethodInfo> assemblyMethods = GetAssemblyMethods(_assemblies);

            return assemblyMethods
                .Where(m => m.MetadataToken == methodInfo.MetadataToken);
        }

        public IEnumerable<InvocationExpressionSyntax> GetMethodInvocations()
        {
            return _methodInvocationModels.Keys;
        }

        public IEnumerable<InvocationExpressionSyntax> GetMethodInvocationsByName(string methodName)
        {
            return GetMethodInvocations()
                .Where(s => s.Expression.GetText().ToString().Contains(methodName));
        }

        public IEnumerable<Type> GetInvocationMethodParameterTypes(InvocationExpressionSyntax invocationExpression)
        {
            SemanticModel model = _methodInvocationModels[invocationExpression];

            var methodSymbol = model.GetSymbolInfo(invocationExpression).Symbol;

            var methodName = methodSymbol.Name;

            Assembly assembly = _assemblies
                .Where(a => methodSymbol.ContainingType.ContainingAssembly.Name == a.GetName().Name)
                .FirstOrDefault();

            List<Type> methodArgumentTypes = ((IMethodSymbol)methodSymbol)
                .Parameters
                .Select(p => assembly.GetType($"{p.Type.ContainingNamespace}.{ p.Type.Name}"))
                .ToList();

            return methodArgumentTypes;

        }

        public SeparatedSyntaxList<ArgumentSyntax> GetInvocationMethodArguments(InvocationExpressionSyntax invocationExpression)
        {
            var arguments = invocationExpression.ArgumentList.Arguments;
            return arguments;
        }

        private void GetSemanticModel(Assembly assembly, out SyntaxTree syntaxTree, out SemanticModel model)
        {
            string projectPath = GetAssemblyProjectPath(assembly);

            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(projectPath).Result;

            project = GetProjectWithSourceFiles(project);

            var compilation = project.GetCompilationAsync().Result;

            syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
            if (syntaxTree == null)
                throw new Exception("Could not locate syntax tree for given assembly.");

            model = compilation.GetSemanticModel(syntaxTree);
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

        private string GetDocumentText()
        {
            return @"using GraphSight.Core;
using System;
            using System.Collections.Generic;
            using System.Text;
            using System.Threading.Tasks;

namespace GraphSight.Core
    {
    public interface IEventTracker
    {
        void TigerGraphDataInsert(IVertex fromVertex, string eventName, IVertex toVertex);
        void TigerGraphDataInsert(IVertex fromVertex, IEdge edge, IVertex toVertex);
        [Event]
        void TigerGraphTrackEvent(IVertex fromVertex, string eventDescription);
        [Event]
        void TigerGraphTrackEvent(IVertex fromVertex, string eventID, string eventDescription);
        [ErrorEvent]
        void TigerGraphTrackError(IVertex fromVertex, Exception exception, string description = "");
    }

        public class GraphSightClient : IEventTracker
        {
            TigerGraphAPIClient _apiClient;
            private Action<Exception> _onErrorAction;
            private Action _onServiceStatusIsDownAction;

            private static readonly int _DEFAULT_RETRIES = 3;
            private static readonly int _DEFAULT_GET_TIMEOUT = 10;
            private static readonly int _DEFAULT_POST_TIMEOUT = 30;

            private string _token;

            public GraphSightClient()
            {
                _apiClient = new TigerGraphAPIClient();
                _apiClient.SetDefaultGetPolicy(_DEFAULT_GET_TIMEOUT);
                _apiClient.SetDefaultPostPolicy(_DEFAULT_POST_TIMEOUT);
                _apiClient.SetMaxRetryPolicy(_DEFAULT_RETRIES);
                _apiClient.SetCircuitBreakerPolicy();
            }

            #region public

            #region Setters
            public GraphSightClient SetUsername(string username)
            {
                _apiClient.SetUsername(username);
                return this;
            }
            public GraphSightClient SetPassword(string password)
            {
                _apiClient.SetPassword(password);
                return this;
            }
            public GraphSightClient SetURI(string uri)
            {
                _apiClient.SetURI(uri);
                return this;
            }
            public GraphSightClient SetSecret(string secret)
            {
                _apiClient.SetSecret(secret);
                return this;
            }
            public GraphSightClient SetGraphName(string graphName)
            {
                _apiClient.SetGraphName(graphName);
                return this;
            }
            public GraphSightClient SetCustomErrorHandler(Action<Exception> action)
            {
                _onErrorAction = action;
                _apiClient.SetCircuitBreakerPolicy(action);
                return this;
            }
            public GraphSightClient SetCustomServiceStatusIsDownAction(Action action)
            {
                _onServiceStatusIsDownAction = action;
                return this;
            }
            public GraphSightClient WithMaxRetries(int maxRetries)
            {
                _apiClient.SetMaxRetryPolicy(maxRetries);
                return this;
            }
            public GraphSightClient WithHttpGetTimeout(int httpGetTimeout)
            {
                _apiClient.SetDefaultGetPolicy(httpGetTimeout);
                return this;
            }
            public GraphSightClient WithHttpPostTimeout(int httpPostTimeout)
            {
                _apiClient.SetDefaultPostPolicy(httpPostTimeout);
                return this;
            }
            #endregion

            #region APICalls
            public async Task<string> PingServerAsync() =>
                await CallAPI(() => { return _apiClient.PingServerAsync(); });

            public string PingServer() => PingServerAsync().Result;

            public async Task<string> RequestTokenAsync() =>
                await CallAPI(() => { return _apiClient.RequestTokenAsync(); });
            public string RequestToken() => RequestTokenAsync().Result;

            public async Task<string> RunQueryAsync(string query) =>
                await CallAPI(() => { return _apiClient.QueryAsync(query); });
            public string RunQuery(string query) => RunQueryAsync(query).Result;

            public async Task<string> RunQueryAsync(string query, Dictionary<string, object> parameters) =>
                await CallAPI(() => { return _apiClient.QueryAsync(query, parameters); });
            public string RunQuery(string query, Dictionary<string, object> parameters) =>
                RunQueryAsync(query, parameters).Result;
            #endregion


            #region Tracking
            public void TigerGraphDataInsert(IVertex fromVertex, string eventName, IVertex toVertex)
            {
                //TODO: Stub
            }

            public void TigerGraphDataInsert(IVertex fromVertex, IEdge edge, IVertex toVertex)
            {
                //TODO: Stub
            }

            public void TigerGraphTrackEvent(IVertex fromVertex, string eventDescription)
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }

            public void TigerGraphTrackEvent(IVertex fromVertex, string eventID, string eventDescription)
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }

            public void TigerGraphTrackError(IVertex fromVertex, Exception exception, string description = "")
            {
                //Dev note: This is supposed to create a new edge/vertex automatically for events
                //TODO: Stub
            }
            #endregion

            #endregion

            #region private

            /// <summary>
            /// Call API is a wrapper for any action calling an APIclient operation. 
            /// This method tries the connection in order to execute user-set error handling delegates. 
            /// </summary>
            private async void CallAPI(Action apiCall)
            {
                ValidateCredentials();
                await Task.Run(() => apiCall());
            }

            /// <summary>
            ///  Call API is a wrapper for any action calling an APIclient operation. 
            /// This method tries the connection in order to execute user-set error handling delegates. 
            /// Function call must contain a return to utilize this method. 
            /// </summary>
            /// <returns>Task of return type</returns>
            private async Task<T> CallAPI<T>(Func<Task<T>> apiCall)
            {
                ValidateCredentials();
                return await apiCall();
            }

            private bool ResponseIsServerStatusError(Exception ex)
            {
                return false;
            }

            private void ValidateCredentials()
            {
                _apiClient.ValidateCredentials();
            }

            private void GetNewTokenIfNotSetAsync()
            {
                if (_token == null)
                    _apiClient.RequestTokenAsync().Wait();
            }

            private void CheckGraphName()
            {

            }
            #endregion
        }
    }
";
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

            foreach (var file in files)
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
