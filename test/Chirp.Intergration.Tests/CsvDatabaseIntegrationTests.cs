using Microsoft.AspNetCore.Mvc.Testing;
using static Chirp;

namespace CSVDBService.Intergration.Tests;
public class CsvDatabaseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CsvDatabaseIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }
    
    [Fact]
    public async Task GetCheepsFromServerAndReadCheeps()
    {

        //Get request to server
        var response = await _client.GetAsync("/cheeps");
        
        //Ensure request code 200
        response.EnsureSuccessStatusCode();
        
        //Get the content, read it into a string
        var content = await response.Content.ReadAsStringAsync();
        
        //Convert the string from json to cheep objects (?)
        var cheep = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cheep>>(content);
        
        //Ensure the list is not null since the file should contain cheeps
        Assert.NotNull(cheep);
    }
    
    [Fact]
    public async Task PostCheepToServerAndReadCheeps()
    {
        var cheepToSend = new Cheep
        {
            Author = Environment.UserName,
            Message = "Intergration test", //Can be converted into fuzzy test then ensure that the random string is actually outputted below
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };
        
        var contentToSend = await _client.PostAsJsonAsync("/cheep", cheepToSend);
        
        contentToSend.EnsureSuccessStatusCode();
        
        var response = await _client.GetAsync("/cheeps");
        
        var content = await response.Content.ReadAsStringAsync();
        var cheeps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cheep>>(content);
        
        //Ensure the cheeps are not null
        Assert.NotNull(cheeps);
        
        //Ensure we have the correct cheep returned to us
        var lastCheep = cheeps.Last();
        Assert.Equal("Intergration test", lastCheep.Message);
        
        
    }
    
}