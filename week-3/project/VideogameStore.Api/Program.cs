using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;
using VideogameStore.Api.Services;
using VideogameStore.Api.DTOs;
using System.Diagnostics;


// initializing the builder
var builder = WebApplication.CreateBuilder(args);

// connection string
var conn_string = "Server=localhost;Database=VideogameStoreDb;User Id=sa;Password=libraryPass1!;TrustServerCertificate=true;";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/VideogameReport.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// set the conn_string to the builder
builder.Services.AddDbContext<VideogameStoreDbContext>(options => options.UseSqlServer(conn_string),
    ServiceLifetime.Scoped, ServiceLifetime.Singleton); // allowing singleton scope

builder.Services.AddDbContextFactory<VideogameStoreDbContext>(options => options.UseSqlServer(conn_string));

// add custom service to builder
builder.Services.AddScoped<IFullfillService, FulfillService>();
builder.Services.AddScoped<BurstPlanner>();
builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<IPromotionService, PromotionService>();

// adding Swagger to builder B)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// build app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Endpoint area
app.MapGet("/", () => "nothing xd");

// get all items (videogames)
app.MapGet("/inventory", async (VideogameStoreDbContext db) =>
{
   return await db.GameStore.ToListAsync();
});

// GET ALL EMPLOYEES FROM EACH STORE
app.MapGet("/employees-per-store/", async (string? search, VideogameStoreDbContext db) =>
{
    var query = db.Stores
        .SelectMany(s => s.Employees);

    if (!string.IsNullOrEmpty(search))
        query = query.Where(e => e.Name.ToLower().Contains(search.ToLower()));
    

    return await query
        .Select(e => new
        {
            e.EmployeeId,
            e.Name,
            e.Address,
            e.Email,
            e.StoreId
        })
        .ToListAsync();
});


// get all customer sales
// if there aren't any sales it will always show null. We need sales first to see something
app.MapGet("/customer-Sales/", async (string? search, VideogameStoreDbContext db) =>
{
    // I want a query for the Sales table but don't go to the database yet. Wait until I provide you any filters
    var query = db.Sales.AsQueryable(); // go directly into the Sales table
    // AsQueryable() method converts an IEnumerable collection into an IQueryable interface.
    

    if (!string.IsNullOrEmpty(search)) // filter user navigation
    {
        query = query.Where(s => s.Customer.Name.ToLower().Contains(search.ToLower()));
    }

    return await query
        .Select(s => new
        {
            s.SaleId,
            s.StoreId,
            s.EmployeeId,
            s.PaymentMethodId,
            CustomerName = s.Customer.Name,
            s.CustomerId,
            s.Format,
            s.SaleDate,
            s.Status,
            s.Priority
        })
        .ToListAsync(); // finally the query gets executed into an only efficient SQL command
});

// method to fulfill one videogame Sale
app.MapPost("/Customer-Sale-Order", async ( 
    GameSalePaylod SaleRequest, 
    IDbContextFactory<VideogameStoreDbContext> factory, 
    CancellationToken ct, 
    IFullfillService fSvc,
    IPromotionService promoSvc) =>
{
    try
    {
        
    // define the email validation
    var resolvedCustomerId = fSvc.ResolveCustomerId(SaleRequest.CustomerEmail);
    
    // calculate the subtotal
    decimal subtotal = SaleRequest.Quantity * SaleRequest.UnitPrice;

    // delegate the complete information to the new service
    var promoResult = await promoSvc.ValidateAndApplyAsync(SaleRequest.PromoCode, SaleRequest.CustomerId, subtotal, ct);

    if (!string.IsNullOrEmpty(SaleRequest.PromoCode) && !promoResult.IsValid)
    {
        return Results.BadRequest(new {message = promoResult.Message });
    }

    await using var db = await factory.CreateDbContextAsync(ct);

    var NewGameSale = new Sale
    {
        CustomerId = SaleRequest.CustomerId,
        StoreId = SaleRequest.StoreId,
        EmployeeId = SaleRequest.EmployeeId,
        PaymentMethodId = SaleRequest.PaymentMethodId,
        Priority = Priority.Normal,
        PromotionId = promoResult.PromotionId,

        SaleDetails = {new Sale_Detail {
            Quantity = SaleRequest.Quantity, 
            VideogameId = SaleRequest.VideogameId, 
            UnitPrice = SaleRequest.UnitPrice}}
    };

    db.Sales.Add(NewGameSale);

    // we want to check if the customer 
    // if the coupon was valid we save the history
    if(promoResult.IsValid && promoResult.PromotionId.HasValue)
    {
        var usageRecord = new Customer_Promotion
        {
            CustomerId = SaleRequest.CustomerId,
            PromotionId = promoResult.PromotionId.Value // .Value is used to get the nullable value
        };
        db.C_Promotions.Add(usageRecord);
    }

    await db.SaveChangesAsync(ct);

    // lets try to fulfill it - stock process
    FulfillResult result = await fSvc.FulfillOneAsync(NewGameSale.SaleId, ct); // creating new game sale (once created it sets it SaleId)

    return Results.Ok(new {
        SaleId = NewGameSale.SaleId, 
        result = result.ToString(),
        Subtotal = subtotal,
        AppliedDiscount = promoResult.Percentage,
        TotalFinal = subtotal - promoResult.Percentage
        });
    } catch (CustomerNotFoundException ex)
    {
        Log.Warning("Sale rejected: {Email} not found", ex.Email);
        return Results.BadRequest(new { message = ex.Message, email = ex.Email });

    } catch (Exception ex)    
    {
        Log.Error(ex, "Unexpected error creating sale");
        return Results.Problem("Unexpected error.");
    }
});

app.MapPost("/sales/burst", (int n, bool expedited,ISeeder seeder, 
    IServiceScopeFactory scopes, IHostApplicationLifetime lifetime) =>
{
    var ids = seeder.SeedSales(n, expedited); // calling the seed sales method with the stuff from front end
    var appStopping = lifetime.ApplicationStopping; // gives us a cancellation token that is called when app goes to shutdown

    _ = Task.Run( async () => // assigning the task result to a discard runs this as a background task
    {
        try
        {
            using var scope = scopes.CreateScope(); // ask for a fresh scope
            var service  = scope.ServiceProvider.GetRequiredService<IFullfillService>();
            await service.FulfillBurstAsync(ids, appStopping);
        } 
        catch (Exception ex)
        {   
            // This task is fire and forget because we aren't waiting or storing its result
            // any exceptions would be "swallowed" i.e. they would die with the task in the background 
            Log.Error(ex, "Burst fulfillment failed");
        }
    }, appStopping);

});

app.MapPost("/benchmark", async (int n, IFullfillService fs, ISeeder seeder, CancellationToken ct) =>
{
    // Lets see how sequential vs concurrent/arallel runs compare - with mixed orders
    var ids1 = seeder.ResetAndCreateSales(n);

    // First, sequential
    var sw1 = Stopwatch.StartNew(); // start our stopwatch

    foreach ( var id in ids1)
        await fs.FulfillOneAsync(id, ct);

    sw1.Stop();

    // Next concurrent
    var ids2 = seeder.ResetAndCreateSales(n);

    var sw2 = Stopwatch.StartNew(); // start second stopwatch
    await fs.FulfillBurstAsync(ids2, ct);
    sw2.Stop();

    return new
    {
        sequentialMs = sw1.ElapsedMilliseconds,
        concurrentMs = sw2.ElapsedMilliseconds,
        speedupFactor = sw2.ElapsedMilliseconds == 0 
            ? 0 
            : (double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds
    };
});

app.MapGet("/reports/top-customers", async (VideogameStoreDbContext db) =>
{
    var ranked = await db.Sales
        .GroupBy(s => s.CustomerId)
        .Select(g => new { CustomerId = g.Key, OrderCount = g.Count() })
        .OrderByDescending(x => x.OrderCount)
        .ToListAsync();

    return Results.Ok(ranked);
});

// Search by product
app.MapGet("/reports/rank-of/{videogameId:int}", async (int videogameId, VideogameStoreDbContext db) =>
{
    // 1. Hash-based lookup: totales por juego (Dictionary, O(1) para encontrar el nuestro)
    var totalsByGame = await db.Sale_Details
        .GroupBy(sd => sd.VideogameId)
        .Select(g => new { VideogameId = g.Key, Units = g.Sum(l => l.Quantity) })
        .ToDictionaryAsync(x => x.VideogameId, x => x.Units);

    if (!totalsByGame.TryGetValue(videogameId, out int myUnits))
        return Results.NotFound(new { message = $"VideogameId {videogameId} has no sales." });

    // 2. Sorted array para binary search: O(log n)
    var unitsDesc = totalsByGame.Values.OrderByDescending(u => u).ToArray();

    var index = Array.BinarySearch(unitsDesc, myUnits, Comparer<int>.Create((a, b) => b.CompareTo(a)));

    return Results.Ok(new { videogameId, unitsSold = myUnits, rank = index >= 0 ? index + 1 : -1 });
});

app.MapGet("/reports/fulfillment-rate", async (VideogameStoreDbContext db) =>
{
    var total = await db.Sales.CountAsync();
    var fulfilled = await db.Sales.CountAsync(s => s.Status == Status.Fulfilled); // ajusta al nombre real de tu enum
    var backordered = await db.Sales.CountAsync(s => s.Status == Status.Backordered);

    return Results.Ok(new
    {
        total,
        fulfilled,
        backordered,
        fulfillmentRate = total == 0 ? 0 : (double)fulfilled / total
    });
});

app.Run(); 
Log.CloseAndFlush(); // ensures that all batched or buffered log events are written to their final destinations
