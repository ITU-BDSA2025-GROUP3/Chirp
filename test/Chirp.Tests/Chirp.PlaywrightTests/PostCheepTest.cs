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

        
        await Page.FillAsync("#Input_Email", "joe@itu.dk");
        await Page.FillAsync("#Input_Password", "Test123!"); 
        await Page.ClickAsync("button[type=submit]");

        await Expect(Page).ToHaveURLAsync(new Regex(".*Timeline.*", RegexOptions.IgnoreCase));

        
        string message = "Playwright test cheep " + DateTime.Now.ToString("HHmmss");
        await Page.FillAsync("textarea[name='text']", message);

       
        await Page.ClickAsync("input[type=submit], button:has-text('Chirp!')");

        
        var cheepList = Page.Locator("#messagelist");
        await Expect(cheepList).ToContainTextAsync(message);
    }
}