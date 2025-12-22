//NOTE:
//These tests require a running Chirp.Web instance.
//By default, they expect the app to be running at http://localhost:5273.
//But if your app is running on a different port, set:
//export CHIRP_BASE_URL="http://localhost:<your-port>"

using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chirp.Tests.Chirp.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UICheeps : PageTest
{
    private static readonly string BaseUrl =
    Environment.GetEnvironmentVariable("CHIRP_BASE_URL") ?? "http://localhost:5273";
    private const string UserName = "joe";
    private const string Password = "Test123!";

    private async Task LoginAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        await Page.FillAsync("#Input_UserName", UserName);
        await Page.FillAsync("#Input_Password", Password);

        await Page.ClickAsync("button[type=Submit]");
        
        await Expect(Page).ToHaveURLAsync($"{BaseUrl}/");
    }

    [Test]
    public async Task CheepInput_VisibleOnly_WhenUserIsLoggedIn()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        
        var cheepInput = Page.Locator("input[name='Message']");
        Assert.False(await cheepInput.IsVisibleAsync(),
            "Cheep input should not be visible when user is not authenticated.");
        
        await LoginAsync();
        
        await Page.GotoAsync($"{BaseUrl}/");
        cheepInput = Page.Locator("input[name='Message']");
        Assert.True(await cheepInput.IsVisibleAsync(),
            "Cheep input should be visible when user is authenticated.");
    }

    [Test]
    public async Task CheepLongerThan160_IsNotStored()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/");

        var cheepInput = Page.Locator("input[name='Message']");
        Assert.True(await cheepInput.IsVisibleAsync(), "Cheep input must be visible for this test.");

        var longCheep = new string('a', 161);

        var initialMatches = await Page.Locator($"li:has-text(\"{longCheep}\")").CountAsync();

        await cheepInput.FillAsync(longCheep);

        var cheepForm = Page.Locator("div.cheepbox form").First;
        await cheepForm.Locator("button[type='submit']").ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var finalMatches = await Page.Locator($"li:has-text(\"{longCheep}\")").CountAsync();

        Assert.AreEqual(initialMatches, finalMatches,
            "Cheeps longer than 160 characters should not be stored or shown in the timeline.");
    }
}