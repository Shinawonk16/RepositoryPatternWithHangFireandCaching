using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    public DbSet<Customer> Customers { get; set; }
}
