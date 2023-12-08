using System.Configuration;
using Application.Abstractions;
using Application.Configuration;
using Domain.Enum;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Repository;
using Persistence.Services;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HangFireConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
);

// builder.Services.AddDbContext<ApplicationDbContext>(
//     option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
// );


// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//                    options.UseMySql(
//                        Configuration.Get("DefaultConnection"),
//                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

// Add services to the container.
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseMemoryStorage());
    builder.Services.AddHangfireServer();
builder.Services.AddHangfire(x => x.UseSqlServerStorage("connectionString"));
var cache = builder.Configuration.GetSection("CacheConfiguration");
builder.Services.Configure<CacheConfiguration>(cache);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



#region 
builder.Services.Configure<CacheConfiguration>(cache);
//For In-Memory Caching
builder.Services.AddMemoryCache();
builder.Services.AddTransient<MemoryCacheService>();
builder.Services.AddTransient<RedisCacheService>();
builder.Services.AddTransient<Func<CacheTech, ICacheService>>(serviceProvider => key =>
{
    switch (key)
    {
        case CacheTech.Memory:
            return serviceProvider.GetService<MemoryCacheService>();
        case CacheTech.Redis:
            return serviceProvider.GetService<RedisCacheService>();
        default:
            return serviceProvider.GetService<MemoryCacheService>();
    }
});
#endregion

#region Repositories
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHangfireDashboard("/jobs");
app.UseAuthorization();

app.MapControllers();

app.Run();
