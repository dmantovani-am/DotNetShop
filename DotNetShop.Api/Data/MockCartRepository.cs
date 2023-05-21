using DotNetShop.Data;
using DotNetShop.Models;

namespace DotNetShop.Api.Data;

public class MockCartRepository : ICartRepository
{
    public Task Add(Product product)
    {
        throw new NotImplementedException();
    }

    public Task Clear()
    {
        throw new NotImplementedException();
    }

    public Task<IList<CartItem>> GetItems()
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetTotal()
    {
        throw new NotImplementedException();
    }

    public Task<int> Remove(Product product)
    {
        throw new NotImplementedException();
    }
}
