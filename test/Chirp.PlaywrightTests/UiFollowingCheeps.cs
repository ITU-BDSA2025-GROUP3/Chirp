using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chirp.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UiFollowingCheeps : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("CHIRP_BASE_URL") ?? "http://localhost:5273";

    private const string Email = "joe";
    private const string Password = "Test123!";

    private const string BellaCheep = "Bella says hello";
    private const string CherylCheep = "Cheryl was here";

    public async Task LoginAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/");

        if (Page.Url.Contains("/Identity/Account/Login"))
        {
            await Page.FillAsync("#Input_UserName", "joe");
            await Page.FillAsync("#Input_Password", "Test123!");
            await Page.ClickAsync("button[type=submit]");
        }

        await Page.WaitForURLAsync($"{BaseUrl}/");
    }


    [Test]
    public async Task Timeline_OnlyShowsCheepsFromFollowedAuthors_AndUpdatesOnFollowChanges()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/");

        Assert.That(
            await Page.Locator($"li:has-text('{BellaCheep}')").CountAsync(),
            Is.GreaterThan(0)
        );

        Assert.That(
            await Page.Locator($"li:has-text('{CherylCheep}')").CountAsync(),
            Is.EqualTo(0)
        );

        await Page.GotoAsync($"{BaseUrl}/Authors/Cheryl");
        await Page.ClickAsync("button:has-text('Follow')");
        await Page.GotoAsync($"{BaseUrl}/");

        Assert.That(
            await Page.Locator($"li:has-text('{CherylCheep}')").CountAsync(),
            Is.GreaterThan(0)
        );

        await Page.GotoAsync($"{BaseUrl}/Authors/Bella");
        await Page.ClickAsync("button:has-text('Unfollow')");
        await Page.GotoAsync($"{BaseUrl}/");

        Assert.That(
            await Page.Locator($"li:has-text('{BellaCheep}')").CountAsync(),
            Is.EqualTo(0)
        );
    }
}
