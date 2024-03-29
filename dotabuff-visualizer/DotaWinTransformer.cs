using DotabuffVisualizer.Data;
using DotabuffVisualizer.Models;
using System.Text.Json;

namespace DotabuffVisualizer;

public class DotaWinTransformer
{
    private Dictionary<string, (int, DotaItem.Type)> _cachedItems = null;
    private static readonly HttpClient _client = new();
    private readonly string[] boots = { "boots", "greaves", "treads" };
    private readonly string[] excluded = { "Aegis of the Immortal", "Clarity", "Dust of Appearance",
    "Sentry Ward", "Bottle", "Aghanim's Shard", "Aghanim's Scepter" };
    private readonly string _apiKey;

    // "B479F4855E8EC7C228DF9045FA77978B"
    public DotaWinTransformer(string apiKey)
    {
        _apiKey = apiKey;
    }

    public IAsyncEnumerable<DotaWinItem> TransformItems(DotabuffHero hero)
    {
        return hero.Items.Where(i => !excluded.Contains(i.Name)).Select(i =>
        {
            var (price, type) = _cachedItems[i.Name];
            var addedWinrate = i.Winrate - hero.Winrate;
            return new DotaWinItem
            {
                Name = i.Name,
                Winrate = i.Winrate,
                Matches = i.Matches,
                Price = price,
                Type = type,
                AddedWinrate = Math.Round(addedWinrate, 2),
                WinratePer1000Gold = Math.Round(addedWinrate / price * 1000, 2)
            };
        });
    }

    public async Task<Dictionary<string, (int, DotaItem.Type)>> CacheItemsAsync()
    {
        var url = @"https://api.steampowered.com/IEconDOTA2_570/GetGameItems/v1/?format=JSON&language=en_us&key=" + _apiKey;
        var jsonString = await _client.GetStringAsync(url);
        var obj = JsonSerializer.Deserialize<DotaItemsList>(jsonString);
        var itemsList = obj.result;
        _cachedItems = itemsList.items
        .Where(i => i.recipe == 0 && i.localized_name != "Necronomicon")
        .Select(i =>
        {
            if (i.localized_name == "Dagon")
            {
                var level = i.name.Last();
                if (level != 'n')
                {
                    i.localized_name = $"Dagon (level {level})";
                }
            }

            var item = new
            {
                Name = i.localized_name,
                Price = i.cost,
            };

            return item;
        })
        .ToDictionary(i => i.Name, i =>
        {
            DotaItem.Type itemType;
            if (i.Price <= 0)
            {
                itemType = DotaItem.Type.Neutral;
            }
            else if (boots.Any(b => i.Name.Contains(b, StringComparison.OrdinalIgnoreCase)))
            {
                itemType = DotaItem.Type.Boots;
            }
            else { itemType = DotaItem.Type.Core; }

            return (i.Price, itemType);
        });

        return _cachedItems;
    }
}
