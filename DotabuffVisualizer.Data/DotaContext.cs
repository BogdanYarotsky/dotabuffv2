namespace DotabuffVisualizer.Data;
using Microsoft.EntityFrameworkCore;

public class DotaContext : DbContext
{
    public DbSet<DotaUpdateLog> UpdateLogs { get; set; }
    public DbSet<DotaItem> Items { get; set; }
}
