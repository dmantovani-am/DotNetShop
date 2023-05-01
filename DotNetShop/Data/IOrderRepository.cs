namespace DotNetShop.Data;

public interface IOrderRepository
{
    Task Add(Order order);
}
