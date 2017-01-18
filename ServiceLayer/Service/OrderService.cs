using System.Threading.Tasks;
using DAL.Entities;
using ServiceLayer.Repository;

namespace ServiceLayer.Service
{
    public class OrderService : EntityService, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IEntityRepository entityRepository,
            IOrderRepository orderReoiRepository) : base(entityRepository)
        {
            _orderRepository = orderReoiRepository;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderRepository.CreateOrderAsync(order);
        }
    }
}