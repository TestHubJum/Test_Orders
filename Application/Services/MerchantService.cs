using Application.Abstractions;
using Application.Models.Merchants;
using Domain;
using Domain.Entities;

namespace Application.Services
{
    public class MerchantService(OrdersDbContext context) : IMerchantService 
    {
        public async Task<MerchantDto> Create(MerchantDto merchant)
        {
            var entity = new MerchantEntity
            {
                Name = merchant.Name,
                Phone = merchant.Phone,
                WebSite = merchant.WebSite
            };

            var result = await context.Merchants.AddAsync(entity);
            var resultEntity = result.Entity;

            await context.SaveChangesAsync();

            return new MerchantDto
            {
                Id = resultEntity.Id,
                Name = resultEntity.Name,
                Phone = resultEntity.Phone,
                WebSite = resultEntity.WebSite
            };

        }
    }
}
