// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.Net.Http.Headers;
using System.Net.Http.Json;


public class Chirp //Modifier should NOT be public, need proper internal visibility status!
{
    

    public static int Main(string[] args)
    {
        //HTTP CLIENT SETUP
        var baseURL = "https://bdsagroup03chirpremotedb-bgaghsguh8cdhdaq.swedencentral-01.azurewebsites.net/";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);
        
        var rootCommand = new RootCommand("chirp");

        //hej
        
        //Create the read command
        var readCommand = new Command("read", "Read cheeps and display to terminal");
        var readLimit = new Argument<int>("limit")
        {
            Description = "Limit on the number of latest cheeps to read",
            DefaultValueFactory = parseResult => 5
        };
        readCommand.Arguments.Add(readLimit);
        rootCommand.Subcommands.Add(readCommand);
        
        //Runs an async operation awaiting the response of our /cheeps endpoint
        readCommand.SetAction(async parseResult =>
        {
            
            var cheepRequest = await client.GetFromJsonAsync<IList<Cheep>>("/cheeps");
            if (cheepRequest != null)
            {
                UserInterface.PrintCheeps(cheepRequest.ToList());   
            }
        });
        //Create the cheep command
        var cheepCommand = new Command("cheep", "Cheep a message");
        var cheepArgument = new Argument<string>("message") {Description = "The message to be cheeped"};
        
        cheepCommand.Arguments.Add(cheepArgument);
        rootCommand.Subcommands.Add(cheepCommand);
        
        cheepCommand.SetAction(async parseResult =>
        {
            var message = parseResult.GetValue(cheepArgument)!; // '!' used to assert non-null msg val
            Cheep cheep = stringToCheep(message);
            
            //Send webrequest to store the cheep
            var postCheep = await client.PostAsJsonAsync("/cheep", cheep);
            
            //Ensure we get a proper success code
            postCheep.EnsureSuccessStatusCode();
            
            UserInterface.PrintCheeps(cheep);
        });
        

        return rootCommand.Parse(args).Invoke();
    }

    private static Cheep stringToCheep(string msg)
    {
        var record = new Cheep
        {
            Author = Environment.UserName,
            Message = msg,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        return record;
    }

    public record Cheep
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
}