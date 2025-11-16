using Microsoft.EntityFrameworkCore;

namespace JumbotronEventFinder.Data
{
    public class JumbotronEventFinderContext : DbContext
    {
        public JumbotronEventFinderContext(DbContextOptions<JumbotronEventFinderContext> options)
            : base(options)
        {
        }

        public DbSet<JumbotronEventFinder.Models.Show> Show { get; set; } = default!;
        public DbSet<JumbotronEventFinder.Models.Category> Category { get; set; } = default!;
        public DbSet<JumbotronEventFinder.Models.Purchase> Purchase { get; set; } = default!;
    }
}
