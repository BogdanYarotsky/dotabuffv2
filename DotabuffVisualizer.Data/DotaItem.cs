namespace DotabuffVisualizer.Data;

public class DotaItem
{
    // id will be here later



    // unique index
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public Type ItemType { get; set; }
    public enum Type
    {
        Boots = 0, Core, Neutral
    }
}
