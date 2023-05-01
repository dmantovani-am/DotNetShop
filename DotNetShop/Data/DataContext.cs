using DotNetShop.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetShop.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    { }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<CartItem> CartItems { get; set; }
}

