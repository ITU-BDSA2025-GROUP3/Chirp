using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Chirp.Tests.Chirp.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PostCheepTests : PageTest
{
    [Test]
    public async Task UserCanPostCheep_AndSeeItInTimeline()
    {
      
        await Page.GotoAsync("http://localhost:5273/Identity/Account/Login");

        
        await Page.FillAsync("#Input_UserName", "elisa");
        await Page.FillAsync("#Input_Password", "Oreo-12345");
        await Page.ClickAsync("button[type=submit]");

        await Expect(Page).ToHaveURLAsync("http://localhost:5273/");
        
        string message = "Playwright test cheep " + DateTime.Now.ToString("HHmmss");
        await Page.FillAsync("#Message", message);

       
        await Page.ClickAsync("input[type=submit], button:has-text('Submit')");

        
        var cheepList = Page.Locator("#messagelist");
        await Expect(cheepList).ToContainTextAsync(message);
    }
}