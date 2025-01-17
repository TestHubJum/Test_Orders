using Application.Models.Carts;

namespace Application.Models.Orders
{
    public class CreatOrderDto
    {
        public string? Name { get; set; }
        public long OrderNumber { get; set; }
        public long CustomerId { get; set; }
        public CartDto? Cart { get; set; }
        public long MerchantId { get; set; }
    }
}
