using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JumbotronEventFinder.Models;

namespace JumbotronEventFinder.Data
{
    public class JumbotronEventFinderContext : DbContext
    {
        public JumbotronEventFinderContext (DbContextOptions<JumbotronEventFinderContext> options)
            : base(options)
        {
        }

        public DbSet<JumbotronEventFinder.Models.Show> Show { get; set; } = default!;
        public DbSet<JumbotronEventFinder.Models.Category> Category { get; set; } = default!;
        public
            DbSet<Purchase> Purchase
        { get; set; } = default!;
    }
}
