using Application.Models.Merchants;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMerchantService
    {
        Task<MerchantDto> Create (MerchantDto merchant);
    }
}
