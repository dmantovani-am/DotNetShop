namespace DotNetShop.Data;

public interface IRepository<T>
    where T : class, IHasId
{
    Task Add(T item);

    Task Delete(int id);

    IAsyncEnumerable<T> GetAll();

    Task<T?> Get(int id);
}
