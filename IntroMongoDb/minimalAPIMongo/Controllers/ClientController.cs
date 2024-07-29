using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<User> _user;

        public ClientController(MongoDbService mongoDbService)
        {
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<ActionResult<List<Client>>> Get()
        {
            try
            {
                var clients = await _client.Find(FilterDefinition<Client>.Empty).ToListAsync();
                return Ok(clients);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(Client client)
        {
            try
            {
                var userExists = await _user.Find(u => u.id == client.UserId).AnyAsync();
                if (!userExists)
                {
                    return BadRequest(new { Message = "User not found" });
                }

                await _client.InsertOneAsync(client);
                return StatusCode(201, client);
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
                var result = await _client.DeleteOneAsync(x => x.Id == id);

                if (result.DeletedCount == 0)
                {
                    return NotFound(new { Message = "Client not found" });
                }

                return StatusCode(204); 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<Client>> Update(Client client)
        {
            try
            {
                var userExists = await _user.Find(u => u.id == client.UserId).AnyAsync();
                if (!userExists)
                {
                    return BadRequest(new { Message = "User not found" });
                }

                var result = await _client.ReplaceOneAsync(x => x.Id == client.Id, client);

                if (result.ModifiedCount == 0)
                {
                    return NotFound(new { Message = "Client not found" });
                }

                return StatusCode(200, client);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetById(string id)
        {
            try
            {
                var filter = Builders<Client>.Filter.Eq(c => c.Id, id);
                var client = await _client.Find(filter).FirstOrDefaultAsync();

                if (client == null)
                {
                    return NotFound(new { Message = "Client not found" });
                }

                return Ok(client);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
