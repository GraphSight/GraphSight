using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphSight.Core.GraphBuilders
{
    /// <summary>
    /// This class is used to iterate a user's namespace to dynamically build a graph schema. 
    /// This allows for the return of a schema as a graph query or loading data directly into class objects without
    /// the user having to define definitions. 
    /// </summary>
    internal class NamespaceGraphSchemaBuilder
    {
        public bool HasErrorHandling { get; set; }
        public bool HasEventTracking { get; set; }

        private NamespaceAnalyzer _namespaceIterator;

        public NamespaceGraphSchemaBuilder() 
        {
            _namespaceIterator = new NamespaceAnalyzer(); 
        }

        public void CheckNamespace(Assembly assembly) 
        {
            NamespaceAnalyzer namespaceIterator = new NamespaceAnalyzer();

            var eventMethods = typeof(IEventTracker).GetMethods();

            //TODO: 
            //First, call the namespaceiterator GetCallerNamespaceTypesImplementingInterface<T> to find ALL class implementing 
            //either IVertex or IEdge types.
            //Using these classes, create new graph nodes. Use namespace iterator GetCallerNamespaceTypesContainingAttribute
            //to fill in the attributes and names of each vertex and edge. 

            //For each event method from IEventTracker, we want to call namespaceIterator.GetCallerNamespaceMethodReferences().Count()  
            //on any items containing Event or ErrorEvent attributes. If the count is > 0, set HasErrorHandling/HasEventTracking, then use
            //the iterator to get the references, find the vertex types, and generate new graph nodes for containing data. 

            //we can also get method references like this example, in case we need this:
            //namespaceIterator.GetCallerNamespaceMethodReferences(typeof(GraphSightClient).GetMethod("TrackEvent", new Type[] { typeof(IVertex), typeof(string) }));
            //^^ Basically this would get an instance of that call with TWO DEFINED PARAMETERS.

        }
    }
}
