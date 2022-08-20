namespace DotabuffVisualizer.Models;

using DotabuffVisualizer.Data;

public class DotaWinItem
{
    public string Name { get; set; }
    public int Matches { get; set; }
    public double Winrate { get; set; }
    public int Price { get; set; }
    public double WinratePer1000Gold { get; set; }
    public double AddedWinrate { get; set; }
    public DotaItem.Type Type { get; set; }
}