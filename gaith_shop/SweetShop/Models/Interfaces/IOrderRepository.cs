using SweetShop.Models;

namespace SweetShop.Models.Interfaces;

public interface IOrderRepository
{
    void CreateOrder(Order order);
}
