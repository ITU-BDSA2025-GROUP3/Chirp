using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using SimpleDB;


using Chirp;



namespace Chirp.Integration.Tests
{
    // Custom factory that ensures CSVDatabase uses a temporary file
    public class CustomWebApplicationFactory 
    // starts the Chirp app in memory for integration testing
        : WebApplicationFactory<Program>, IDisposable
    {
        private string _tempFile;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Create a unique temp file for this test run
            _tempFile = Path.Combine(AppContext.BaseDirectory, $"chirp_cli_db_{Guid.NewGuid()}.csv");

            builder.ConfigureServices(services =>
            {
                // Force CSVDatabase to use our temp file
                var db = CSVDatabase<Cheep>.Instance;
                db.setFilePath(_tempFile);
            });
        }

        public void Dispose()
        {
            // Delete the temp file when the test is finished
            if (_tempFile != null && File.Exists(_tempFile))
                File.Delete(_tempFile);
        }
    }

    public class CsvDatabaseIntegrationTests 
        : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CsvDatabaseIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RootEndpoint_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        //delete file permenantly
        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}