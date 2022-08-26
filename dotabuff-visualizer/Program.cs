using ConsoleTables;
using DotabuffVisualizer;
using DotabuffVisualizer.Data;

Console.SetWindowSize(70, 35);

await using var crawler = await DotabuffCrawler.StartAsync();

var patch = await crawler.GetPatchVersionAsync();
Console.WriteLine(patch);
return;

var transformer = new DotaWinTransformer("B479F4855E8EC7C228DF9045FA77978B");
await transformer.CacheItemsAsync();

while (true)
{
    Console.WriteLine("Hero name?");
    var heroName = Console.ReadLine();
    var hero = await crawler.ExtractDotabuffHeroInfo(heroName);
    Console.WriteLine(hero.HeroName);
    Console.WriteLine(hero.Winrate);
    var items = await transformer.TransformItems(hero).ToListAsync();

    ConsoleTable.From(items.Where(i => i.Type == DotaItem.Type.Boots && !i.Name.Contains("Travel"))
                            .Select(i => new { i.Name, i.Matches, i.AddedWinrate })
                            .OrderByDescending(i => i.AddedWinrate))
                            .Configure(o => { o.EnableCount = false; }).Write();

    ConsoleTable.From(items.Where(i => i.Type == DotaItem.Type.Core && i.Price > 1000)
                            .Select(i => new { i.Name, i.Matches, i.AddedWinrate, i.WinratePer1000Gold })
                            .OrderByDescending(i => i.Matches).Take(10)
                            .OrderByDescending(i => i.WinratePer1000Gold))
                            .Configure(o => { o.EnableCount = false; }).Write();

    Console.ReadKey();
    Console.Clear();
}