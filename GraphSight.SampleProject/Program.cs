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
                username: "", 
                password: "", 
                URI: "graphsight.i.tgcloud.io", 
                secret: "");

            string resp = client.PingServer();
            var a = "";


        }
    }
}
