using Application.Abstractions;
using Application.Mappers;
using Application.Models.Orders;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class OrdersService(OrdersDbContext context, ICartsService cartsService) : IOrdersService
    {
        public async Task<OrderDto> Create(CreatOrderDto order)
        {
            var OrderByOrderNumber = await context.Orders.FirstOrDefaultAsync(x => 
                x.OrderNumber == order.OrderNumber && x.MerchantId == order.MerchantId);

            if(OrderByOrderNumber != null) 
            { 
                throw new DuplicateEntityException($"Order with orderNumber {order.OrderNumber} already exists for merchant " +
                                                    $"{order.MerchantId}");
            }

            if(order.Cart == null)
            {
                throw new ArgumentNullException();
            }

            var cart = await cartsService.Create(order.Cart);

            var entity = new OrderEntity
            {
                OrderNumber = order.OrderNumber,
                Name = order.Name,
                CustomerId = order.CustomerId,
                CartId = cart.Id
            };

            
            var orderSaveResult = await context.Orders.AddAsync(entity);
            await context.SaveChangesAsync();

            var orderEntityResult = orderSaveResult.Entity;

            return  orderEntityResult.ToDto();
        }

   

        public async Task<OrderDto> GetById(long orderId)
        {
            var entity = await context.Orders
                .Include(o => o.Cart)
                .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (entity == null)
            {
                throw new EntityNotFoundExceptions($"Order entity with id {orderId} not found");
            }

            return entity.ToDto();
        }

        public async Task<List<OrderDto>> GetByUser(long customerId)
        {
            var entity = await context.Orders
                .Include(o => o.Cart)
                .ThenInclude(c => c.CartItems)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync(); 

            return entity.Select(x => x.ToDto()).ToList();
        }

        public async Task<List<OrderDto>> GetAll()
        {
            var entity = await context.Orders
                .Include(o => o.Cart)
                .ThenInclude(c => c.CartItems)
                .ToListAsync(); 

            return entity.Select(x => x.ToDto()).ToList();
        }

        //todo: статусную модель для отмены
        public Task Reject(long orderId)
        {
            throw new NotImplementedException();
        }
    }
}
