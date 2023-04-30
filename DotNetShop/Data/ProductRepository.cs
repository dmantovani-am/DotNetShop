using Microsoft.EntityFrameworkCore;

namespace DotNetShop.Data;

class ProductRepository : DataContextRepository<Product>, IProductRepository
{
    public ProductRepository(DbContext dataContext) : base(dataContext)
    { }

    public async Task<IEnumerable<Product>> GetTopProducts(int n = 9)
    {
        var products = await GetAll().ToListAsync();
        return products.OrderBy(p => p.Price).Take(n);
    }
}
