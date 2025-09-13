using CarAPI.Data;
using CarAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Use In-Memory DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("DemoDb"));

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen();           // Registers Swagger generator

var app = builder.Build();

// Configure and seed in-memory DB
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var person = new Person { Name = "Alice", Email = "alice@example.com" };
    var car = new Car { Model = "Civic", Make = "Honda", Year = 2020, Price = 22000, Color = "Blue", Person = person };
    var purchase = new Purchase { Buyer = person, Car = car, PurchaseDate = DateTime.UtcNow };

    context.People.Add(person);
    context.Cars.Add(car);
    context.Purchases.Add(purchase);
    context.SaveChanges();
}

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                         // Generates Swagger JSON
    app.UseSwaggerUI();                       // Serves Swagger UI
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
