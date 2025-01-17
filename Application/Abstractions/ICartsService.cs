using Application.Models.Carts;

namespace Application.Abstractions
{
    public interface ICartsService
    {
        Task<CartDto> Create(CartDto cart);
    }
}
