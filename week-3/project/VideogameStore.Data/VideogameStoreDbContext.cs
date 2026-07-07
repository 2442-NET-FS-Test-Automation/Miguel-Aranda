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

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Sale_Detail>(e =>
        {
            e.Property(p => p.UnitPrice).HasColumnType("decimal(10,2)");
        });

        b.Entity<Store>(e =>
        {
                e.HasOne(s => s.Employee)
                    .WithOne()
                    .HasForeignKey<Store>(s => s.EmployeeId);
        });

        b.Entity<Sale>(e =>
        {
                e.HasOne(s => s.Store)
                    .WithMany()
                    .HasForeignKey(s => s.StoreId)
                    .OnDelete(DeleteBehavior.Restrict); // restriction similar to on delete cascade

                e.HasOne(s => s.Employee)
                    .WithMany()
                    .HasForeignKey(s => s.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict); // restriction similar to on delete cascade
        });

        b.Entity<Videogame_Store>(e =>
        {
                e.HasOne(vs => vs.Videogame).WithMany().HasForeignKey(vs => vs.VideogameId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(vs => vs.Store).WithMany().HasForeignKey(vs => vs.StoreId).OnDelete(DeleteBehavior.Restrict);
        });

        // setting row version to EF Core RowVersion
        b.Entity<Videogame>().Property(i => i.RowVersion).IsRowVersion();
        b.Entity<Customer>().Property(c => c.Email).HasMaxLength(256);
        b.Entity<Customer>().HasIndex(c => c.Email).IsUnique();

        b.Entity<Employee>().HasData(
            new Employee { EmployeeId = 1, Name = "Juan", SurName = "Lopez", Address = "Harmond street 134", Email="Juan@example.com"},
            new Employee { EmployeeId = 2, Name = "Mario", SurName = "Rosa", Address = "Julieth street 3", Email="Mario@example.com"}
        );

        b.Entity<Videogame>().HasData(
            new Videogame { VideogameId = 1, Gamename = "Super Smash Bros Ultimate", 
            Genre = "Fighting", Rating = Rating.Everyone, Stock = 10},
            new Videogame { VideogameId = 2, Gamename = "The Legend Of Zelda Ocarina Of Time", 
            Genre = "Adventure", Rating = Rating.Teen, Stock = 6},
            new Videogame { VideogameId = 3, Gamename = "Hollow Knight: Silksong", 
            Genre = "2D Platformer", Rating = Rating.Teen, Stock = 5}
        );

        b.Entity<Store>().HasData(
            new Store { StoreId = 1, StoreName = "Gamestop", Address="James Bond street 77", EmployeeId = 1}, 
            new Store { StoreId = 2, StoreName = "Gamestop 2", Address="Abraham's lincon street 44", EmployeeId = 2}
        );   

        b.Entity<Videogame_Store>().HasData(
            new Videogame_Store {Videogame_StoreId = 1, StoreId = 1, VideogameId = 1 },
            new Videogame_Store {Videogame_StoreId = 2, StoreId =2, VideogameId = 2 }
        );

    }
}
