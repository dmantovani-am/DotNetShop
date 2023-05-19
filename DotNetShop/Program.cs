using DotNetShop.Data;
using DotNetShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("(default)") ?? throw new InvalidOperationException("Connection string 'DataContextConnection' not found.");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    var configuration = builder.Configuration;
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

AddRepositories(builder.Services, builder.Configuration);
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
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();

void AddRepositories(IServiceCollection services, ConfigurationManager configuration)
{
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<ICartRepository, CartRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IContactRepository, ContactRepository>();
}