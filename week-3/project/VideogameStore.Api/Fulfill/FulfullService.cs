using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;
namespace VideogameStore.Api.Fullfill;
public interface IFullfillService
{
    public Task<FulfillResult> FulfillOneAsync(int orderid, CancellationToken ct);
    public Task<BurstResult> FulfillBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);

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

    public async Task<FulfillResult> FulfillOneAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        //var order = await db.Orders.Include(o => o.Lines).FirstAsync(o => o.Id == orderId, ct); // LINQ with async
        var order = await db.Game.Include(o => o.)
        
    }
}