using GraphSight.Core;
using GraphSight.Core.Client;
using GraphSight.Core.Converters.TigerGraph;
using GraphSight.Core.Enums.TigerGraph;
using GraphSight.Core.Graph;
using GraphSight.Core.Graph.JSON;
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
           
            //**Client Configuartion**

            //Client Configuration: Here we can submit API calls or track events. 
            GraphSightClient client = new GraphSightClient()
                .SetUsername("tigergraph")
                .SetPassword("123456")
                .SetURI("1000.i.tgcloud.io")
                .SetSecret("dmbthm12dhugsaa211tvoj0u9aqaj2dt")
                //.WithMaxRetries(3)
                //.WithHttpGetTimeout(15)
                //.WithHttpPostTimeout(45)
                .SetCustomErrorHandler((exception) => Console.WriteLine(exception.Message));

            //We can call several API calls on this client. More to come later.
            client.PingServer();
            client.RequestToken();
            client.Upsert("sample data");
            client.RunQuery("sample interpreted query");

            //Alternatively these requests can be called asynchronously. 
            Task.Run(() => client.PingServerAsync());


            //**Generating a schema**: 
            //We can obtain the query to generate the schema for our entire project by calling the static analyzer:

            TigerSchemaGraph schema = TigerGraphAnalyzer.AnalyzeCodeAndGenerateSchema("TestGraph");
            string schemaQuery = TigerGraphAnalyzer.AnalyzeCodeAndGenerateSchemaAsQuery("TestGraph");

            //This reads all classes that implement the IVertex or IEdge interface and generates the schema accordingly, 
            //and it also dynamically creates Event and Error vertices/edges for any of the client 'Track' calls. 
            //I.E, if you say "TigerGraphTrackError(SampleVertex, Exception)", it will automatically generate a "SampleVertex_Exception" 
            //vertex and a "SampleVertex_Throws_Exception" edge in the schema. This handles type conversions too. 
            //NOTE: There is one line of code that is broken for this in actual usage environments, so it currently won't work until that line is fixed. 
            //However, feel free to take a look at how the code works. 


            //To see how the generated schema would create a query, here is a manually-built schema to demonstrate: 
            TigerSchemaGraph sampleSchema = new TigerSchemaGraph("PhoneApp");

            TigerSchemaVertex User = new TigerSchemaVertex("User", "UserID", PrimaryIDTypes.STRING);
            User.AddAttribute(new TigerSchemaAttribute("Name", AttributeTypes.STRING, defaultValue: "Bob"));

            TigerSchemaVertex Button = new TigerSchemaVertex("Button", "ButtonName", PrimaryIDTypes.STRING);

            TigerSchemaEdge Clicks = new TigerSchemaEdge("User_Clicks_Button", isDirected: true, reverseEdge: "Button_Clicked_By");
            Clicks.AddAttribute(new TigerSchemaAttribute("Timestamp", AttributeTypes.DATETIME));

            Clicks.AddSourceTargetPair(User, Button);

            sampleSchema.AddVertex(User);
            sampleSchema.AddVertex(Button);
            sampleSchema.AddEdge(Clicks);

            string asQuery = sampleSchema.GetGraphQuery();


            //**Creating and tracking data**
            //Sample Standard Data Vertices
            UserVertex user = new UserVertex() {
                UserID = "1",
                Name = "Bob"
            };
            AccountVertex account = new AccountVertex()
            { 
                 AccountID = "100", 
                 Status = "Active"
            };
            UserHasAccountEdge userHasAccount = new UserHasAccountEdge()
            {
                DateCreated = DateTime.Today
            };

            //This would upload the data to the schema. Note: Does not work due to issues with Upsert conversion, see below
            client.TigerGraphDataInsert(user, userHasAccount, account);


            //Event Tracking
            //When certain events happen in our code, data from these calls will populate event or error nodes for the vertex passed in. 
            client.TigerGraphTrackError(user, new Exception("User App Broke!"));
            client.TigerGraphTrackEvent(user, eventID: "1", eventDescription: "Clicked Help Button");
            client.TigerGraphTrackSequence(user, sequenceID: "1001", sequenceNumber: "1", description: "User entered Login Screen");
            client.TigerGraphTrackSequence(user, sequenceID: "1001", sequenceNumber: "2", description: "User entered Account Screen");


            //Above it was mentioned that the upsert call is not currently working. The Client's Track functions would
            //call this set of instructions. The json string here is built from the user-defined nodes and edges, however the 
            //format is slightly off, so the tracking events will not work until this part is fixed: 
            SchemaToJsonConverter converter = new SchemaToJsonConverter();
            JsonUpsertFormat result = converter.GetSourceDestinationFormat(user, userHasAccount, account);
            string jsonString = JsonConvert.SerializeObject(result);
        }
    }

    
} 
