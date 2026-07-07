using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;
namespace VideogameStore.Api.Fullfill;
public interface IFullfillService
{
    public Task<FulfillResult> FulfillOneAsync(int saleid, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> saleids, CancellationToken ct);

}
public enum FulfillResult {Fulfilled, Backordered}
public record BurstResult (int Fulfilled, int Backordered);
public class FulfillService : IFullfillService
{
    private readonly IDbContextFactory<VideogameStoreDbContext> _factory;
    private readonly BurstPlanner _planner;

    public FulfillService(IDbContextFactory<VideogameStoreDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
    } 

    public async Task<FulfillResult> FulfillOneAsync(int saleid, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        // obtain sales (including it's details) and then fulfill the saleDetails list
        var sale = await db.Sales
            .Include(s => s.SaleDetails)
            .FirstAsync(s => s.SaleId == saleid, ct);
        
        // dictionary with videogame id and quantity
        var requested = sale.SaleDetails.ToDictionary(sd => sd.VideogameId, sd => sd.Quantity);

        bool canFullfill = true;

        foreach(Sale_Detail detail in sale.SaleDetails)
        {
            Videogame videogame = await db.Game
                .FirstAsync(v => v.VideogameId == detail.VideogameId);

            if(videogame.Stock < detail.Quantity)
            {
                canFullfill = false;
                break;
            }

            videogame.Stock -= detail.Quantity;
        }

        // but what if we can't fulfill?
        if (!canFullfill)
        {
            // if can't fulfill its backordered
            // sale.Status = Status.Backordered;
            Log.Warning("Sale {saleid} rejected: [out of STOCK!]", saleid);
            return FulfillResult.Backordered;
        }

        // retry save method
        if(!await SaveWithRetryAsync(db, requested, ct))
        {
            db.ChangeTracker.Clear();
            Log.Warning("Sale {saleid} failed after many concurrency attempts!", saleid);
            return FulfillResult.Backordered;
        }
        
        Log.Information("Sale completed: {saleid}, {LineCount} games", saleid, sale.SaleDetails.Count);
        return FulfillResult.Fulfilled;

    }

    private static async Task<bool> SaveWithRetryAsync(
        VideogameStoreDbContext db, IReadOnlyDictionary<int, int> requestedByGameId, CancellationToken ct)
    {
        int attempts = 0;
        while (true)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            
            } 
            catch(DbUpdateConcurrencyException ex) when (attempts < 3)
            {
                attempts++; // we count attempts
                Log.Warning("Concurrency exception detected! attempted {attempts} times", attempts);

                foreach(var entry in ex.Entries)
                {
                    // obtain the real database values
                    var CurrentDatabaseValues = await entry.GetDatabaseValuesAsync(ct);

                    // if someone deleted the game while processing we cancel the operation
                    if(CurrentDatabaseValues == null) return false;

                    // we update the "OriginalValues" of the Change Tracker to tell
                    // EF Core: I know what changed, now compare me with that new data
                    entry.OriginalValues.SetValues(CurrentDatabaseValues);

                    if(entry.Entity is Videogame videogame)
                    {
                        // we obtain the stock recently gotten from the db
                        int freshStock = CurrentDatabaseValues.GetValue<int>(nameof(Videogame.Stock));
                        
                        // we look for how many units the user would like to buy
                        int desiredAmount = requestedByGameId[videogame.VideogameId];
                        
                        // EXIT CONDITION: If the stock in db is out of reach to buy, it fails.
                        if(freshStock < desiredAmount) return false;
                        
                        videogame.Stock = freshStock - desiredAmount;
                    }
                }
            }
            catch(DbUpdateConcurrencyException)
            {
                // if this reaches out is because attempts >= 3. We exited the loop and we failed
                Log.Error("Maximum number of concurrency tries has exceeded");
                return false;
            }
        }    
    }

    public async Task<BurstResult> FulfillBurstAsync(IEnumerable<int> saleids, CancellationToken ct)
    {
        List<int> idList = saleids.ToList();

        List<Sale> sales;

        await using (var db = await _factory.CreateDbContextAsync(ct))
        {
            sales = await db.Sales.Where(o => idList.Contains(o.SaleId)).ToListAsync();
        }

        // we send the Sale list to the planner
        var planned = _planner.OrderByPriority(sales);

        // we respect the planner priorities
        var tasks = planned.Select(id => FulfillOneAsync(id, ct));

        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfillResult.Fulfilled),
            Backordered: results.Count(r => r == FulfillResult.Backordered)
        );
    }
}