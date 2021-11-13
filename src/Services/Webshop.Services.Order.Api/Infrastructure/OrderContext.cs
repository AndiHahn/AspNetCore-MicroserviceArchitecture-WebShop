using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webshop.Services.Order.Api.Domain.OrderAggregate;

namespace Webshop.Services.Order.Api.Infrastructure
{
    public class OrderContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;

        public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            this.loggerFactory = loggerFactory ?? throw new System.ArgumentNullException(nameof(loggerFactory));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.OrderAggregate.Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address);
            modelBuilder.Entity<Domain.OrderAggregate.Order>().HasOne(o => o.Status).WithMany().HasForeignKey(o => o.StatusId);

            modelBuilder.Entity<OrderItem>().HasKey(o => o.Id);

            modelBuilder.Entity<OrderStatus>().HasKey(o => o.Id);
        }
    }
}
