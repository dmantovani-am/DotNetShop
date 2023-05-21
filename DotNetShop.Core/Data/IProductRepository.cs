namespace DotNetShop.Data;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetTopProducts(int n = 9);
}
