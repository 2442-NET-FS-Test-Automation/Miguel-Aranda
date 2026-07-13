using Library.Data.Entities;
namespace LibraryData;

using Library.Data;
using Microsoft.EntityFrameworkCore;
public class InventoryRepository : IInventoryRepository
{
    // Our repo class needs a db context we ask for a dbcontext form ASP.NET DI CContrainer
    // same pattern we've been using since day 1 of the minimal API
    private readonly IDbContextFactory<LibraryDbContext> _factory;

    public InventoryRepository(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    // Lets make some CRUD
    // Actually pretty simple to do - because we don't have to concern ourselves with business logic checks etc.
    // All we write is DB access stuff

    // Let's write some Read methods
    // Get all InventoryItems

    public async Task<IReadOnlyList<InventoryItem>> GetAllASync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i => i.Product).ToListAsync();
    }

    // Get item by it's SKU
    public async Task<InventoryItem?> GetInventoryItemSkuAsync(string sku)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Inventory.Include(i => i.Product).FirstOrDefaultAsync(i => i.Product.Sku == "sku");
    }

    // Lets do a simple add
    //
    public async Task<InventoryItem> AddInventoryItemAsync(string sku, string name, decimal price, int quantity)
    {
        await using var db = await _factory.CreateDbContextAsync();
        InventoryItem newItem = new InventoryItem
        {
            Product = new Product {Sku = sku, Name = name, Price = price},
            CurrentStock = quantity
        };
        db.Inventory.Add(newItem);
        await db.SaveChangesAsync();
        return newItem; // because newItem is an object tracked by EF Core - EF will grab the PK for us
    }

    // lets do a remove
    public async Task<bool> RemoveBySkuAsync(string sku)
    {
        await using var db = await _factory.CreateDbContextAsync();
        // first find the thing we want out of the database - grab
        InventoryItem? itemToRemove = await db.Inventory.Include(i => i.Product)
                                            .FirstOrDefaultAsync(i => i.Product.Sku == sku);

        // Don't assume the search criteria produced a result - check for a null
        // if it's null we failed to remove it - because it didn't exist
        if (itemToRemove is null)
        {
            return false;
        }
        // telling EF we want to remote this object from the DB
        db.Products.Remove(itemToRemove.Product); // this SHOULD cascade 

        await db.SaveChangesAsync();
        return true;
    }
}