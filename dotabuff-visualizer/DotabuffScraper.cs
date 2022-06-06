using Microsoft.Playwright;
using DotabuffVisualizer.Models;

namespace DotabuffVisualizer;

public sealed class DotabuffCrawler : IAsyncDisposable, IDisposable
{
    private const string heroItemsListSelector = "body > div.container-outer.seemsgood > div.skin-container > div.container-inner.container-inner-content > div.content-inner > section > article > table > tbody > tr";
    private const string heroWinrateSelector = "body > div.container-outer.seemsgood > div.skin-container > div.container-inner.container-inner-content > div.header-content-container > div.header-content > div.header-content-secondary > dl:nth-child(2) > dd > span";

    private readonly BrowserTypeLaunchPersistentContextOptions _options = new BrowserTypeLaunchPersistentContextOptions { Headless = true };
    private IPlaywright _playwright;
    private IBrowserContext _browser;
    private IPage _page;

    private DotabuffCrawler() { }
    private async Task<DotabuffCrawler> StartBrowserAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchPersistentContextAsync(Directory.GetCurrentDirectory() + "temp", _options);
        _page = await _browser.NewPageAsync();
        return this;
    }

    public async Task<DotabuffHero> GetHeroItemsAsync(string name)
    {
        var heroUrl = new UriBuilder("https", "www.dotabuff.com/heroes");
        heroUrl.Path = name.ToLower().Replace(' ', '-') + "/items";
        return await ExtractDotabuffHeroInfo(heroUrl.Uri);
    }

    private async Task<DotabuffHero> ExtractDotabuffHeroInfo(Uri heroUrl)
    {
        var page = await _browser.NewPageAsync();
        await page.RouteAsync("**/*", async r =>
        {
            if (r.Request.ResourceType == "image") await r.AbortAsync();
            else await r.ContinueAsync();
        });

        Console.WriteLine("Extracting " + heroUrl);
        await page.GotoAsync(heroUrl.ToString(), new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        string name = await page.Locator("h1").InnerTextAsync();

        // get winrate
        var heroWinrate = await page.Locator(heroWinrateSelector).InnerTextAsync();
        heroWinrate = heroWinrate.Replace("%", "").Trim();

        // get items
        var items = new List<DotabuffItem>();
        var heroList = page.Locator(heroItemsListSelector);
        var itemCount = await heroList.CountAsync();
        // make parallel for faster parsing
        for (int i = 0; i < itemCount; i++)
        {
            var row = heroList.Nth(i);
            var columns = row.Locator("td");
            var itemName = await columns.Nth(1).InnerTextAsync();
            if (itemName.StartsWith("Recipe")) continue;
            var matches = int.Parse(await columns.Nth(2).GetAttributeAsync("data-value"));
            if (matches < 9001) break; // sample is too small
            var winrate = double.Parse(await columns.Nth(3).GetAttributeAsync("data-value"));
            winrate = Math.Round(winrate, 2);
            
            items.Add(new DotabuffItem
            {
                Name = itemName,
                Winrate = winrate,
                Matches = matches
            });
        }

        await page.CloseAsync();
        var heroName = name.Replace("Items", "").Trim();
        return new DotabuffHero { HeroName = heroName, Winrate = double.Parse(heroWinrate), Items = items };
    }

    public static async Task<DotabuffCrawler> CreateAsync()
    {
        var crawler = new DotabuffCrawler();
        return await crawler.StartBrowserAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    public void Dispose()
    {
        _browser.DisposeAsync()
                .GetAwaiter()
                .GetResult();
        _playwright.Dispose();
    }
}