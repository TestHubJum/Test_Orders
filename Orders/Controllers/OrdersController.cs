using Application.Abstractions;
using Application.Models.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Orders.Controllers
{
    [Route("api/orders")]
    public class OrdersController (IOrdersService orders, ILogger<OrdersController> logger) : ApiBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreatOrderDto request)
        {
            //logger.LogInformation($"Method api/orders Create started.\nRequest: {JsonSerializer.Serialize(request)}");
            logger.LogInformation($"Method api/orders Create started.\n");

            var result = await orders.Create(request);

            //logger.LogInformation($"Method api/orders Create finish.\nRequest:{JsonSerializer.Serialize(request)}" + $"Response: {JsonSerializer.Serialize(result)}");
            logger.LogInformation($"Method api/orders Create finish.\n");

            return Ok(result);
        }

        [HttpGet("{orderId:long}")]
        [Authorize]
        public async Task<IActionResult> GetById(long orderId)
        {
            logger.LogInformation($"Method api/orders/{orderId} GetById started.");

            var result = await orders.GetById(orderId);

            logger.LogInformation($"Method api/orders/{orderId} GetById  finish.\n" +
                                    $"Response: {JsonSerializer.Serialize(result)}");

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation($"Method api/orders GetAll started.");

            var result = await orders.GetAll();

            logger.LogInformation($"Method api/orders GetAll  finish. Result count: {result.Count()}");

            return Ok(new {orders = result});
        }
        

        [HttpGet("customers/{customersId:long}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(long customersId)
        {
            logger.LogInformation($"Method api/orders/customers/{customersId} GetByUser started.");

            var result = await orders.GetByUser(customersId);

            logger.LogInformation($"Method api/orders/customers/{customersId} GetByUser finish. Result count: {result.Count()}");

            return Ok(new { orders = result });
        }

        // ненужно
        [HttpPost("{orderId:long}/reject")]
        public async Task<IActionResult> Reject(long orderId)
        {
           
            throw new NotImplementedException();
           
        }
    }
}
