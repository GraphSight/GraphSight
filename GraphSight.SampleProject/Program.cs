using GraphSight.Core;
using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.Graph;
using GraphSight.SampleProject.Vertices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GraphSight.SampleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //GraphSightClient client = new GraphSightClient(
            //    username: "tigergraph", 
            //    password: "123456", 
            //    URI: "https://f43e7c9dc64b45f592a2b9d855852124.i.tgcloud.io", 
            //    secret: "pp4gfpbhrh4nsrq7hm4m7h84ujrc6of1")
            //    .WithMaxRetries(3)
            //    .WithHttpGetTimeout(15)
            //    .WithHttpPostTimeout(45);

            GraphSightClient client = new GraphSightClient()
                .SetUsername("tigergraph")
                .SetPassword("123456")
                .SetURI("5452ce24ca0b44a19690711d87d4f6e8.i.tgcloud.io")
                .SetSecret("dmbthm12mhu1saa231tvoj0u9aqkj2dt")
                .WithMaxRetries(3)
                .WithHttpGetTimeout(15)
                .WithHttpPostTimeout(45)
                .SetCustomErrorHandler((exception) => Console.WriteLine(exception.Message));

            //NamespaceIterator.GetTypesWithAttribute(); 
            //var k = new TigerVertexAttribute()
            //{
            //    Name = "Test Attribute",
            //    Value = new Dictionary<string, string>() { { "a", "b" } }
            //};

            //TigerVertex t = new TigerVertex()
            //{
            //    Name = "Test",
            //    PrimaryId = "1",
            //    PrimaryIdType = PrimaryIDTypes.INT,
            //    Attributes = new List<TigerVertexAttribute>() { 
            //        new TigerVertexAttribute(){ 
            //            Name = "Test Attribute",
            //            Value = new int[]{ 1, 2, 3}
            //        }
            //    }
            //};

            //JsonSerializerSettings settings = new JsonSerializerSettings()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    Formatting = Formatting.Indented
            //};
            //settings.Converters.Add(new TigerJsonValueConverter());

            ////string json = JsonConvert.SerializeObject(k, settings);
            //string json = JsonConvert.SerializeObject(k, Formatting.Indented);
            //Console.WriteLine(json); 
            TestVertex tss = new TestVertex();
            testMethod(tss);

            NamespaceAnalyzer it = new NamespaceAnalyzer();
            var list = it.GetMethodInvocationsByAssembly();
            list = it.GetMethodInvocationsByName(list, "testMethod");
            var kk = list.First().Expression;
            var jj = list.First().ArgumentList; 
            var k = it.GetCallerNamespaceMethodInfos(typeof(Program).GetMethod("test", new Type[] { typeof(int) }));


            //string s = client.PingServer();
            Console.WriteLine(client.PingServer());
            Console.WriteLine(client.RunQuery("CREATE GRAPH everythingGraph (*); CREATE GRAPH emptyGraph()"));
            var a = client.PingServerAsync();
            Console.WriteLine("test");
            Console.WriteLine(a.Result);
            Console.WriteLine(client.RequestTokenAsync().Result);
            test(5);
            client.TigerGraphDataInsert(null, "", null);
        }

        public static void testMethod(TestVertex t) { }
        public static void test(int i) { }
    }

    
} 
