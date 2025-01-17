using Application.Models.Carts;
using Application.Models.Orders;
using Domain.Entities;

namespace Application.Mappers
{
    public static class OrdersMapper
    {
        public static OrderDto ToDto(this OrderEntity entity, CartEntity? cart = null)
        {
           return new OrderDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId!.Value,
                Cart = cart == null ? entity.Cart?.ToDto() : cart.ToDto() ,
                OrderNumber = entity.OrderNumber,
                Name = entity.Name
            };
        }

        public static OrderEntity ToEntity(this CreatOrderDto entity, CartDto? cart = null)
        {
            return new OrderEntity
            {
                CustomerId = entity.CustomerId,
                Cart = cart?.ToEntity(),
                OrderNumber = entity.OrderNumber,
                Name = entity.Name
            };
        }
    }
}
