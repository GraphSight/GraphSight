﻿using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSight.Core.Graph
{
    public class TigerGraph
    {
        public string Name { get; private set; }
        public List<TigerSchemaVertex> Vertices { get; private set; }
        public List<TigerSchemaEdge> Edges { get; private set; }

        private GraphOptions _options;

        public TigerGraph(string name, GraphOptions graphOptions = null)
        {
            _options = graphOptions;

            Name = name;
            Vertices = new List<TigerSchemaVertex>();
            Edges = new List<TigerSchemaEdge>();
        }

        #region PublicMethods
            public TigerGraph AddVertex<T>(string name, string primaryIdName, PrimaryIDTypes primaryIdType)
            {
                if (GetVertexByName(name) == null) {
                    TigerSchemaVertex vertex = new TigerSchemaVertex(name, primaryIdName, primaryIdType);

                    if (Utils.isPrimitive(typeof(T))) 
                    {
                        vertex.AddAttribute(new TigerSchemaAttribute()
                        {
                            DefaultValue = default(T),
                            //Todo: finish off here. Using primitives might not work because we need a name of the attribute. Set t as object and throw if primitive? 
                            // another consideration there would be to potentiall make the attribute data public for name and default type, let the user pass that in, and then 
                            // utilize the values for the actual data load. 
                            //Potentially make another class that is the schema, and leave what we have. 
                        });
                    }

                    Vertices.Add(vertex);
                }
                    

                return this;
            }
            public TigerGraph AddVertexData(string name, object data, OperationCode? opCode = null)
            {
                var vertex = GetVertexByName(name);
                if (vertex == null)
                    throw new Exception("Cannot add data to a non-existent vertex.");


                //Todo: iterate object data using reflection to add attributes, unless the object is a primative type (check with converter)
                //Todo: add way to index vertex

                return this;
            }

            public void AddEdge(string name) { }
            public void AddEdge(string name, TigerSchemaVertex fromVertex, TigerSchemaVertex toVertex) { }
            public void AddEdgeData(string name, object data, OperationCode? opCode = null) { }

            public void ClearVertexData(string name) { }
            public void ClearEdgeData(string name) { }

            #endregion

        #region PrivateMethods
        private TigerSchemaVertex GetVertexByName(string name) => Vertices.Find(v => v.Name == name);

        #endregion
        }
    }

