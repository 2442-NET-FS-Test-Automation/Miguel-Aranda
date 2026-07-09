using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;
using VideogameStore.Api.Fullfill;
using System.Runtime.Intrinsics.X86;


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
app.MapGet("/allgames", async (VideogameStoreDbContext db) =>
{
   return await db.Game.ToListAsync();
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
app.MapPost("/Customer-Sale-Order", async ( GameSalePaylod SaleRequest, 
IDbContextFactory<VideogameStoreDbContext> factory, CancellationToken ct, IFullfillService fSvc) =>
{
    await using var db = await factory.CreateDbContextAsync(ct);

    var NewGameSale = new Sale
    {
        CustomerId = SaleRequest.CustomerId,
        Priority = Priority.Normal,
        SaleDetails = {new Sale_Detail {Quantity = SaleRequest.Quantity,}}
    }; //Sale_DetailId and SaleId are generated automatically so it's now necessary to assign them here

    db.Sales.Add(NewGameSale);

    await db.SaveChangesAsync(ct);

    // lets try to fulfill it
    FulfillResult result = await fSvc.FulfillOneAsync(NewGameSale.SaleId, ct); // creating new game sale (once created it sets it SaleId)
    return Results.Ok(new {SaleId = NewGameSale.SaleId, result = result.ToString()});
});

app.Run(); 
Log.CloseAndFlush(); // ensures that all batched or buffered log events are written to their final destinations
public record GameSalePaylod(int CustomerId, int Quantity, decimal UnitPrice, int VideogameId);

