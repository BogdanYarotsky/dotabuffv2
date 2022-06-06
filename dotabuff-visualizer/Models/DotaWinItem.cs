namespace DotabuffVisualizer.Models;

public class DotaWinItem
{
    public enum Type
    {
        Boots = 0, Core, Neutral
    }

    public string Name { get; set; }
    public int Matches { get; set; }
    public double Winrate { get; set; }
    public int Price { get; set; }

    public double WinratePer1000Gold { get; set; }
    public double AddedWinrate {get; set; }


    public Type ItemType
    {
        get; set;
    }
}