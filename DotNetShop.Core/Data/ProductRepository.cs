using Microsoft.EntityFrameworkCore;

namespace DotNetShop.Data;

public class ProductRepository : DataContextRepository<Product>, IProductRepository
{
    public ProductRepository(DataContext dataContext) : base(dataContext)
    { }

    public async Task<IEnumerable<Product>> GetTopProducts(int n = 9)
    {
        var products = await GetAll().ToListAsync();
        return products.OrderBy(p => p.Price).Take(n);
    }
}
