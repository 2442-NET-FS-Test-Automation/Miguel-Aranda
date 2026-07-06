using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;


// initializing the builder
var builder = WebApplication.CreateBuilder(args);

// connection string
var conn_string = "Server=localhost;Database=LibraryMinimalDb;User Id=sa;Password=libraryPass1!;TrustServerCertificate=true;";

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
// builder.Services.AddScoped<ISomething, Something>()

// adding Swagger to builder B)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// build app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Endpoint area
app.MapGet("/", () => "Hello World!");

// get all items (videogames)
app.MapGet("/inventory", async (VideogameStoreDbContext db) =>
{
   return await db.Game.ToListAsync();
});

app.MapGet("/inventory/id", async (VideogameStoreDbContext db) =>
{
   return db.Game.Include(i => i.Gamename)
        .GroupBy(i => i.Stock >= 5 ? "well-stocked" : "low")
        .Select(g => new{tier = g.Key, count=g.Count(), units=g.Sum(i => i.Stock)})
        .ToList();
});



app.Run();
