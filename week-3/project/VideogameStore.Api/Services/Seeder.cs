using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;


public interface ISeeder
{
    IReadOnlyList<int> SeedSales(int n, bool expedited);
    IReadOnlyList<int> ResetAndCreateSales(int n);
}

public class Seeder : ISeeder
{
    private readonly IDbContextFactory<VideogameStoreDbContext> _factory;
    public Seeder(IDbContextFactory<VideogameStoreDbContext> factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<int> SeedSales(int n, bool expedited)
    {
        using var db = _factory.CreateDbContext();

        // Loading entire entities (avoiding N+1)
        // only games with one entry in GameStore
        var stockedGameIds = db.GameStore.Select(gs => gs.VideogameId).Distinct().ToList();
        var games = db.Game.Where(g => stockedGameIds.Contains(g.VideogameId)).ToList();

        var customerIds = db.Customers.Select(c => c.CustomerId).ToList();
        var storeIds = db.Stores.Select(s => s.StoreId).ToList();
        var employeeIds = db.Employees.Select(e => e.EmployeeId).ToList();
        var paymentMethodIds = db.Payments.Select(p => p.PaymentMethodId).ToList();

        if (!games.Any() || !customerIds.Any() || !storeIds.Any() || !employeeIds.Any() || !paymentMethodIds.Any())
        {
            throw new InvalidOperationException("Cannot seed sales because one or more master tables (Videogame, Customer, Store, Employee, PaymentMethod) are completely empty!");
        }

        var seededSales = new List<Sale>(n);

        for (int i = 0; i < n; i++)
        {
            var randomGame = games[Random.Shared.Next(games.Count)];
            int randomCustomerId = customerIds[Random.Shared.Next(customerIds.Count)];
            int randomStoreId = storeIds[Random.Shared.Next(storeIds.Count)];
            int randomEmployeeId = employeeIds[Random.Shared.Next(employeeIds.Count)];
            int randomPaymentMethodId = paymentMethodIds[Random.Shared.Next(paymentMethodIds.Count)];

            decimal unitPrice = randomGame.Rating == 0 ? 59.99m : 29.99m;

            var sale = new Sale
            {
                CustomerId = randomCustomerId,
                StoreId = randomStoreId,
                EmployeeId = randomEmployeeId,
                PaymentMethodId = randomPaymentMethodId,
                Format = Random.Shared.Next(0, 2) == 0 ? SaleFormat.Physical : SaleFormat.Digital,
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Status = Status.Pending,
                SaleDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                SaleDetails = new List<Sale_Detail>
                {
                    new Sale_Detail
                    {
                        VideogameId = randomGame.VideogameId,
                        Quantity = Random.Shared.Next(1, 4), // quantity between 1 and 3
                        UnitPrice = unitPrice
                    }
                }
            };

            db.Sales.Add(sale);
            seededSales.Add(sale);
        }

        db.SaveChanges();
        return seededSales.Select(s => s.SaleId).ToList();
    }

    public IReadOnlyList<int> ResetAndCreateSales(int n)
    {
        using var db = _factory.CreateDbContext();

        // Si tu entidad Game tiene un campo de stock, lo reseteamos aquí antes de generar ventas nuevas
        var vgStore = db.GameStore.ToList();
        foreach (var game in vgStore)
        {
            game.Stock = Random.Shared.Next(1, 6); // We reset the stock to random values between 1 and 5
        }
        db.SaveChanges();

        // Reutilizamos SeedSales en vez de duplicar toda la lógica de creación
        return SeedSales(n, expedited: false);
    }
}