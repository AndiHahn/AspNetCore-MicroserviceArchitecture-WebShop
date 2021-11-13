using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webshop.Services.Catalog.Api.Models;

namespace Webshop.Services.Catalog.Api.Infrastructure
{
    public class CatalogContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;

        public DbSet<CatalogItem> CatalogItem { get; set; }

        public CatalogContext(
            DbContextOptions<CatalogContext> options,
            ILoggerFactory loggerFactory)
            : base(options)
        {
            this.loggerFactory = loggerFactory ?? throw new System.ArgumentNullException(nameof(loggerFactory));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(loggerFactory)
                .UseSqlite("Filename=CatalogDatabase.db");
        }
    }
}
