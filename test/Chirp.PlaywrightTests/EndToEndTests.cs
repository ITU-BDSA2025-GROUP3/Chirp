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



        //Remember to change login to existing login in database!! 
        //(Extremely poor code to give login details in plain text though)
        await Page.FillAsync("#Input_UserName", "joe");
        await Page.FillAsync("#Input_Password", "Test123!");

    
        await Page.ClickAsync("button[type=Submit]");

        
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/");

      
        await Expect(Page.Locator("h2")).ToContainTextAsync("Timeline");
    }
}