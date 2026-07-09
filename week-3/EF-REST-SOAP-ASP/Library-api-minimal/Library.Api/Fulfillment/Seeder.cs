using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;

// In "production" our orders would come from users. These API's run locally
// so we could either - create a post for a single order and run a shell script or something
// or we could create a seeding endpoint from here to generate some orders for us
public interface ISeeder
{
    IReadOnlyList<int> SeedOrders(int n, bool expedited);
}

public class Seeder : ISeeder
{
    public static readonly string[] Skus = {"BK-001", "BK-002","BK-003"};
    private readonly IDbContextFactory<LibraryDbContext> _factory;
    public Seeder(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }
    public IReadOnlyList<int> SeedOrders (int n, bool expedited)
    {
        // ask for db context
        using var db = _factory.CreateDbContext();

        // create a dictionary based on our product table 
        var pid = db.Products.ToDictionary(p => p.Sku, p => p.Id); // SKU Key, productId value

        // New list of ids
        var ids = new List<int>(n);

        // based on n (number of orders the user want to seed)
        // lets use a for loop to create those orders programatically

        for(int i = 0; i < n; i++)
        {
            var order = new Order
            {
                CustomerId = Random.Shared.Next(1, 3), // random number - bounded
                Priority = expedited ? Priority.Expedited : Priority.Normal,
                Lines = { new OrderLine {ProductId = pid[Skus[i % Skus.Length]], Quantity = 1}}
            };

            db.Orders.Add(order); // Add - state changes in EF CORE change tracker
            db.SaveChanges(); // persists changes
            ids.Add(order.Id); // add the created order's ID to the id list
        }
        return ids;
    }
}