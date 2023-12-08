using Application.Abstractions;
using Domain;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repository;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{   
    private readonly DbSet<Customer> _customer;
    public CustomerRepository(ApplicationDbContext dbContext, Func<CacheTech, ICacheService> cacheService) : base(dbContext, cacheService)
    {
        _customer = dbContext.Set<Customer>();
    }
}
