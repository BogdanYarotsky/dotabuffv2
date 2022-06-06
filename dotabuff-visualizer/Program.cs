// using DotabuffVisualizer;
// using ConsoleTables;
// using DotabuffVisualizer.Models;

// Console.WriteLine("Hero name?");
// var heroName = Console.ReadLine();
// using var crawler = await DotabuffCrawler.CreateAsync();
// var hero = await crawler.GetHeroItemsAsync(heroName);
// Console.WriteLine(hero.HeroName);
// Console.WriteLine(hero.Winrate);
// var transformer = new DotaWinTransformer("B479F4855E8EC7C228DF9045FA77978B");
// var items = await transformer.TransformItems(hero);

// items = items.Where(i => i.AddedWinrate > 0);

// // dto here
// ConsoleTable.From(items.Where(i => i.ItemType == DotaWinItem.Type.Boots)
//                         .Select(i => new { i.Name, i.Matches, i.AddedWinrate })
//                         .OrderByDescending(i => i.AddedWinrate))
//                         .Configure(o => { o.EnableCount = false; }).Write();

// // Console.WriteLine("Neutral");
// // ConsoleTable.From(items.Where(i => i.ItemType == DotaWinItem.Type.Neutral).OrderByDescending(i => i.AddedWinrate)).Write();

// // dto here
// ConsoleTable.From(items.Where(i => i.ItemType == DotaWinItem.Type.Core)
//                     .Select(i => new { i.Name, i.Matches, i.AddedWinrate, i.WinratePer1000Gold })
//                     .OrderByDescending(i => i.Matches).Take(10)
//                     .OrderByDescending(i => i.WinratePer1000Gold))
//                     .Configure(o => { o.EnableCount = false; }).Write();