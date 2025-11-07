using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Chirp.Tests.Chirp.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class LoginTests : PageTest
{
    [Test]
    public async Task UserCanLoginAndSeeTimeline()
    {
       
        await Page.GotoAsync("http://localhost:5273/Identity/Account/Login");

    
     
    }
}