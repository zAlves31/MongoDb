using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using minimalAPIMongo.ViewModels;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _order;
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<Product> _product;

        public OrderController(MongoDbService mongoDbService)
        {
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                var orders = await _order.Find(FilterDefinition<Order>.Empty).ToListAsync();
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(OrderViewModel newOrder)
        {
            try
            {
                Order order = new Order();
                order.Id = newOrder.Id;
                order.Date = newOrder.Date;
                order.Status = newOrder.Status;
                order.ProductId = newOrder.ProductId;
                order.ClientId = newOrder.ClientId;

                var clientOwner = _client.Find(c => c.Id == newOrder.ClientId).FirstOrDefaultAsync();

                if (clientOwner is not null)
                {
                    order.Client = await clientOwner;
                }
                else
                {
                    return NotFound("Cliente nao encontrado");
                }

                var lista = new List<Product>();

                foreach (var productId in newOrder.ProductId!)
                {
                    var item = _product.Find(p => p.id == productId).FirstOrDefault();

                    if (item is not null)
                    {
                        lista.Add(item);
                    }
                }

                order.Products = lista;

                await _order.InsertOneAsync(order);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _order.DeleteOneAsync(x => x.Id == id);

                if (result.DeletedCount == 0)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                return StatusCode(204); 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Order>> Update(Order order)
        {
            try
            {
                var result = await _order.ReplaceOneAsync(x => x.Id == order.Id, order);

                if (result.ModifiedCount == 0)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                return StatusCode(200, order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
                var order = await _order.Find(filter).FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}