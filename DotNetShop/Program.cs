using DotNetShop.Data;
using DotNetShop.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

AddRepository(builder.Services, builder.Configuration);
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

void AddRepository(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddDbContext<DataContext>(options => options.UseSqlite(configuration.GetConnectionString("(default)")));
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<ICartRepository, CartRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
}