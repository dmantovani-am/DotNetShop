using DotNetShop.Data;
using DotNetShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("(default)") ?? throw new InvalidOperationException("Connection string 'DataContextConnection' not found.");

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        var configuration = builder.Configuration;
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

AddRepositories();

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

await AddAdmin();

app.Run();

void AddRepositories()
{
    var services = builder.Services;
    var configuration = builder.Configuration;

    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<ICartRepository, CartRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IContactRepository, ContactRepository>();
    services.AddScoped<ITokenRepository, TokenRepository>();
}

// Crea di default un utente "Admin".
async Task AddAdmin()
{
    // I servizi IUserStore e UserManager richiedono uno scope.
    var services = app!.Services!.CreateScope().ServiceProvider;

    var configuration = new User();
    app.Configuration.Bind("Admin", configuration);

    var usersManager = services.GetRequiredService<UserManager<IdentityUser>>();

    var user = new IdentityUser() { UserName = configuration.UserName! };
    if ((await usersManager.CreateAsync(user, configuration.Password!)).Succeeded)
    {
        await usersManager.AddClaimAsync(user, new("Role", Role.Admin));
    }
}

record User
{
    public string? UserName { get; set; }

    public string? Password { get; set; }
}