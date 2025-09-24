using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using static Chirp;

namespace CSVDBService.Integration.Tests;

// Creates a custom test server for integration testing
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    // Holds the path of the temporary CSV file
    private string? _tempFile;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Create a unique temporary file inside bin/Debug/net9.0
        // This file is used during tests instead of the permanent CSV
        _tempFile = Path.Combine(AppContext.BaseDirectory, $"chirp_cli_db_{Guid.NewGuid()}.csv");

        builder.ConfigureServices(services =>
        {
            // Here you can override or replace services if needed
            // For example, inject your database service to use _tempFile
        });
    }

    // Clean up when the factory is disposed (after all tests are done)
    public void Dispose()
    {
        // Delete the temporary file if it still exists
        if (_tempFile != null && File.Exists(_tempFile))
            File.Delete(_tempFile);
    }
}

// Defines the integration tests that use the custom test server
public class CsvDatabaseIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    // The test client is created from the custom factory
    public CsvDatabaseIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCheepsFromServerAndReadCheeps()
    {
        // Call the GET /cheeps endpoint
        var response = await _client.GetAsync("/cheeps");
        response.EnsureSuccessStatusCode();

        // Read and deserialize the response into a list of Cheep objects
        var content = await response.Content.ReadAsStringAsync();
        var cheep = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cheep>>(content);

        // Verify that the result is not null
        Assert.NotNull(cheep);
    }

    [Fact]
    public async Task PostCheepToServerAndReadCheeps()
    {
        // Create a new cheep object to send
        var cheepToSend = new Cheep
        {
            Author = Environment.UserName,
            Message = "Integration test",
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Send it to the POST /cheep endpoint
        var postResponse = await _client.PostAsJsonAsync("/cheep", cheepToSend);
        postResponse.EnsureSuccessStatusCode();

        // Get the list of cheeps again
        var response = await _client.GetAsync("/cheeps");
        var content = await response.Content.ReadAsStringAsync();
        var cheeps = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cheep>>(content);

        // Verify the list is not null and contains the new message
        Assert.NotNull(cheeps);
        Assert.Equal("Integration test", cheeps!.Last().Message);
    }
}
