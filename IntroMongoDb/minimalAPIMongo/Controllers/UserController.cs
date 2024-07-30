using BCrypt.Net; // Adiciona a biblioteca BCrypt
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using minimalAPIMongo.ViewModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _user;

        public UserController(MongoDbService mongoDbService)
        {
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
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
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

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
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado" });
            }

            return StatusCode(204); 
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateObject(string id, UserUpdateViewModel updatedUser)
        {
            var existingUser = await _user.Find(x => x.id == id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return NotFound(new { Message = "Usuário não encontrado" });
            }

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            await _user.ReplaceOneAsync(x => x.id == id, existingUser);
            return Ok(existingUser);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.id, id);
                var user = await _user.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { Message = "Usuário não encontrado" });
                }

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(string email, string password)
        {
            try
            {
                var user = await _user.Find(u => u.Email == email).FirstOrDefaultAsync();

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return Unauthorized(new { Message = "Credenciais inválidas" });
                }

                return Ok(new { Message = "Autenticação bem-sucedida" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}
