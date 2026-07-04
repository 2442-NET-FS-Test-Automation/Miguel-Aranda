using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Data.Entities;

// this is my API program.cs
// No main. We can think of it as 2 sections
// Registering things with the builder.
// And then configuring things on the app
// And at the very bottom that app object that represents our entire API calls its run method

// Builder area
var builder = WebApplication.CreateBuilder(args);


// The first thing that we need is to give our builder a connection string to our database
var conn_string = "Server=localhost;Database=LibraryMinimalDb;User Id=sa;Password=libraryPass1!;TrustServerCertificate=true;";

// Tell the builder to use our libraryDbContext with the connection string above
// By registering our DbContext class (or even classes, technically you use one per Database)
// We hand off tphe managing of creating and destroying these DbContext objects to ASP.NET's
// dependency injection container. Like spring beans if you're familiar.
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(conn_string));

//Swagger stuff added to builder
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// app area
var app = builder.Build();

// Swagger stuff added to app
app.UseSwagger();
app.UseSwaggerUI();

// Endpoint area
app.MapGet("/", () => "Hello World!");

// Get all items from the inventory
app.MapGet("/inventory", async (LibraryDbContext db) => {
    // we should probably await this - may not matter because we are local
    return await db.Inventory.ToListAsync();
});

// Lets use LINQ - Language Integrated Query
// LINQ is a library that just lets us query collections
// The logic actually flows from SQL DQL - You can use method OR sql query syntax
// You can even save the queries themselves as C# objects if you want to
app.MapGet("/inventory/by-value", (LibraryDbContext db) =>
{
   return db.Inventory.Include(i => i.Product)
        .GroupBy(i => i.CurrentStock >= 5 ? "well-stocked": "low")
        .Select(g => new{ tier = g.Key, count=g.Count(), units = g.Sum(i => i.CurrentStock)})
        .ToList();
});

// Any endpoints that start with "/peek/" are diagnosis/demo
// We are going to use them to expose things like EF Core change tracking and other
// underlying behaviours for learning. A real app would have no reason to expose HTTP endpoints
// to outside users to make this stuff observable.

app.MapGet("/peek/tracking", (LibraryDbContext db) =>
{
   // lets see the underlying EF Core change tracker
   var unchanged = db.Products.First(); // grab the first object. Read but not modified => Unchanged
   var modified = db.Products.Skip(1).First(); // queried... still Unchanged as of here

   modified.Price += 1;

   // When we create a new object and call the dbset's .Add() method it's state is
   // "Added" - this has not actually hit the database yet. But it's tracked to be added.
   db.Products.Add(new Product{ Sku = "BK-TMP", Name="TMP", Price = 1m});

   // This bit of code is the non-production demo bit
   // We are accessing the libraryDbContext object's change tracker to pull info.
   // At most you'd debug with this.
   var states = db.ChangeTracker.Entries()
        .Select(e => new {entity = e.Entity.GetType().Name, state = e.State.ToString()})
        .ToList();

    // Clearing the change tracker manually
    db.ChangeTracker.Clear();
});

// My file always ends with app.Run() - minimal API or Controller API
app.Run();
