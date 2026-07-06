using Microsoft.EntityFrameworkCore;
using VideogameStore.Data.Entities;
using System.Dynamic;

namespace VideogameStore.Data;
public class VideogameStoreDbContext : DbContext
{
    public VideogameStoreDbContext(DbContextOptions<VideogameStoreDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PaymentMethod> Payments => Set<PaymentMethod>();
    public DbSet<Sale_Detail> Sale_Details => Set<Sale_Detail>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Videogame_Store> GameStore => Set<Videogame_Store>();
    public DbSet<Videogame> Game => Set<Videogame>();

    
}
