using Microsoft.EntityFrameworkCore;

namespace Shop.Sales.API.Domain
{
    public class PurchaseContext : DbContext
    {
        public PurchaseContext(DbContextOptions<PurchaseContext> options) : base(options)
        {
        }

        public DbSet<Purchase> Purchases { get; set; }
    }
}
