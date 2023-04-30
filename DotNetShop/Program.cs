using DotNetShop.Data;
using DotNetShop.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

AddRepository(builder.Services);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

void AddRepository(IServiceCollection services)
{
    var options = new DbContextOptionsBuilder<DataContext>()
        .UseSqlite("Data Source=DotNetShop.sqlite;")
        .Options;

    var dataContext = new DataContext(options);
    dataContext.Database.EnsureCreated();

    services.AddScoped<DataContext>(_ => dataContext);
    services.AddScoped<IProductRepository>(_ => new ProductRepository(dataContext));
    services.AddScoped<IRepository<Category>>(_ => new DataContextRepository<Category>(dataContext));
    services.AddScoped<ICartRepository>(CartRepository.GetCart);
}