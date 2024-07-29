using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _user;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService"></param>
        public UserController(MongoDbService mongoDbService)
        {
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                var users = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(User user)
        {
            try
            {
                await _user.InsertOneAsync(user);
                return StatusCode(201, user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _user.FindOneAndDeleteAsync(x => x.id == id);

            return StatusCode(201);
        }

        [HttpPut]
        public async Task<ActionResult<User>> UpdateObject(User p)
        {
            await _user.ReplaceOneAsync(x => x.id == p.id, p);

            return StatusCode(201);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(p => p.id, id);


                var product = await _user.Find(filter).FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound(new { Message = "Produto não encontrado" });
                }

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}
