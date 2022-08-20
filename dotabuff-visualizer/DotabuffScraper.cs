using DotabuffVisualizer.Models;
using Microsoft.Playwright;

namespace DotabuffVisualizer;

public sealed class DotabuffCrawler : IAsyncDisposable
{
    private const string heroItemsListSelector = "body > div.container-outer.seemsgood > div.skin-container > div.container-inner.container-inner-content > div.content-inner > section > article > table > tbody > tr";
    private const string heroWinrateSelector = "body > div.container-outer.seemsgood > div.skin-container > div.container-inner.container-inner-content > div.header-content-container > div.header-content > div.header-content-secondary > dl:nth-child(2) > dd > span";

    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    private DotabuffCrawler() { }
    private async Task<DotabuffCrawler> StartBrowserAsync()
    {
        // takes longest time
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync();
        _context = await _browser.NewContextAsync();
        await _context.RouteAsync("**/*", async r =>
        {
            if (r.Request.ResourceType == "document") await r.ContinueAsync();
            else await r.AbortAsync();
        });
        _page = await _context.NewPageAsync();
        return this;
    }

    public async Task<DotabuffHero> ExtractDotabuffHeroInfo(string name)
    {
        var url = $"https://www.dotabuff.com/heroes/{name.ToLower().Replace(' ', '-')}/items";
        await _page.GotoAsync(url);

        // get hero winrate
        var heroWinrateText = await _page.Locator(heroWinrateSelector).InnerTextAsync();
        double.TryParse(heroWinrateText.Replace("%", ""), out var heroWinrate);

        // get items
        var rows = _page.Locator(heroItemsListSelector);
        var items = GetItemsAsync(rows, heroWinrate);
        return new DotabuffHero { HeroName = name.ToUpper(), Winrate = heroWinrate, Items = items };
    }

    private async IAsyncEnumerable<DotabuffItem> GetItemsAsync(ILocator rows, double heroWinrate)
    {
        for (int i = 0; ; i++)
        {
            var row = rows.Nth(i);
            var columns = row.Locator("td");

            int.TryParse(await columns.Nth(2).GetAttributeAsync("data-value"), out var matches);
            if (matches < 9001) break; // don't go further

            double.TryParse(await columns.Nth(3).GetAttributeAsync("data-value"), out var winrate);
            if (winrate - heroWinrate < 0) continue;

            var itemName = await columns.Nth(1).InnerTextAsync();
            if (itemName.StartsWith("Recipe")) continue;

            yield return new DotabuffItem
            {
                Name = itemName,
                Winrate = Math.Round(winrate, 2),
                Matches = matches
            };
        }
    }

    public static async Task<DotabuffCrawler> StartAsync()
    {
        var crawler = new DotabuffCrawler();
        return await crawler.StartBrowserAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

}