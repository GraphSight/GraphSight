using GraphSight.Core;
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
                .SetURI("https://f43e7c9dc64b45f592a2b9d855852124.i.tgcloud.io")
                .SetSecret("pp4gfpbhrh4nsrq7hm4m7h84ujrc6of1")
                .WithMaxRetries(3)
                .WithHttpGetTimeout(15)
                .WithHttpPostTimeout(45);

            string s = client.PingServer(); 
            Console.WriteLine(client.PingServer());
            var a = client.PingServerAsync();
            Console.WriteLine("test");
            Console.WriteLine(a.Result);
            Console.WriteLine(client.RequestTokenAsync().Result);
            Console.ReadLine();
        }
    }
}
