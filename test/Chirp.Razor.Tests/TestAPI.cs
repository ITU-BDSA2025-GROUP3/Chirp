using Microsoft.AspNetCore.Mvc.Testing;

using Xunit.Abstractions;

namespace Chirp.Razor.Tests;

public class TestAPI : IClassFixture<RazorPageWebAppFactory<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public TestAPI(RazorPageWebAppFactory<Program> fixture, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Theory]
    [InlineData("/", true)] //defaults to page 1
    [InlineData("/?page=1", true)] //page 1 exists
    [InlineData("/?page=0", false)] //zeroth page doesn't exist and user is made aware in bad request
    [InlineData("/?page=-1", false)] //negative pages don't exist and user is made aware in bad request
    [InlineData("/?page=2147483648", false)] //page out of bounds
    [InlineData("/?page=-2147483649", false)] //page out of bounds
    [InlineData("/?pgae=1",  true)] //pgae is not a known query keyword and is ignored
    [InlineData("/?abc", true)] //pgae is not a known query keyword and is ignored
    [InlineData("/?pgae", true)] //pgae is not a known query keyword and is ignored
    public async Task Test_GetRequest_Public(string endpoint, bool expected)
    {
        //act
        try
        {
            var response = await _client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(expected);
        }
        catch (HttpRequestException ex)
        {
            _testOutputHelper.WriteLine(ex.Message);
            Assert.False(expected);
        }
    }

    [Theory]
    [InlineData("Helge", "", "Hello, BDSA students!")]
    [InlineData("Helge", "?page=1", "Hello, BDSA students!")]
    [InlineData("Adrian", "","Hej, velkommen til kurset.")]
    [InlineData("Adrian", "?page=1", "Hej, velkommen til kurset.")]
    public async Task Test_Get_UserTimeline(string author, string query, string message)
    {
        var response = await _client.GetAsync($"/{author}/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("<ul id=\"messagelist\" class=\"cheeps\">", content);
        Assert.Contains(author, content);
        Assert.Contains(message, content);
        Assert.DoesNotContain("There are no cheeps so far.", content);
    }
    [Fact]
    public async Task Test_Get_UserTimeLine_NoCheeps()
    {
        const string author = "THIS_AUTHOR_DOES_NOT_EXIST"; //test db mustn't contain this author
        var response = await _client.GetAsync($"/{author}/"); 
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("There are no cheeps so far.", content);
    }
}