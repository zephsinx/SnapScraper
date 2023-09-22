using Microsoft.EntityFrameworkCore;

namespace SnapScraper;

public class SnapScraperDbContext : DbContext
{
    private readonly string databasePath;
    public SnapScraperDbContext(string databasePath)
    {
        this.databasePath = databasePath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($@"DataSource={this.databasePath};");
        optionsBuilder.EnableDetailedErrors();
    }

    public DbSet<DbCard> Cards { get; set; } = null!;
}