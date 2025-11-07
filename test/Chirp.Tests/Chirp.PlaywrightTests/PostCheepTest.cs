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
       
    }
}