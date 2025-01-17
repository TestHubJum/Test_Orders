using Application.Abstractions;
using Application.Models.Merchants;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Orders.Controllers
{
    [Route("api/merchant")]
    public class MerchantController(IMerchantService merchant, ILogger<OrdersController> logger) : ApiBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create(MerchantDto request)
        {
            logger.LogInformation($"Method api/merchant Create started.\nRequest: {JsonSerializer.Serialize(request)}");
            

            var result = await merchant.Create(request);

            logger.LogInformation($"Method aapi/merchant Create finish.\nRequest:{JsonSerializer.Serialize(request)}" + $"Response: {JsonSerializer.Serialize(result)}");
            

            return Ok(result);
        }
    }
}
