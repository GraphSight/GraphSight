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
            GraphSightClient client = new GraphSightClient(
                username: "tigergraph", 
                password: "", 
                URI: "https://f43e7c9dc64b45f592a2b9d855852124.i.tgcloud.io", 
                secret: "");

            string resp = client.PingServer();
            var a = "";


        }
    }
}
