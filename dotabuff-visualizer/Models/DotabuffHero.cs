namespace DotabuffVisualizer.Models;

public class DotabuffHero
{
    public string HeroName { get; set; }
    public double Winrate { get; set; }
    public List<DotabuffItem> Items { get; set; }
}
