using CarAPI.Data;
using CarAPI.Models;
using CarAPI.Services;
using CarAPI.Services.Interfaces;
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

// add the service layer
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();

var app = builder.Build();

// create some data in the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Create persons
    var person1 = new Person { Name = "Alice", Email = "alice@example.com" };
    var person2 = new Person { Name = "Bob", Email = "bob@example.com" };

    // Create cars
    var car1 = new Car { Model = "Civic", Make = "Honda", Year = 2020, Price = 22000, Color = "Blue", Person = person1 };
    var car2 = new Car { Model = "Corolla", Make = "Toyota", Year = 2019, Price = 18000, Color = "White" };
    var car3 = new Car { Model = "Model 3", Make = "Tesla", Year = 2021, Price = 35000, Color = "Red" };

    // Create a purchase
    var purchase = new Purchase
    {
        Buyer = person1,
        Car = car1,
        PurchaseDate = DateTime.UtcNow
    };

    // Add to context
    context.People.AddRange(person1, person2);
    context.Cars.AddRange(car1, car2, car3);
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
app.MapControllers();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
