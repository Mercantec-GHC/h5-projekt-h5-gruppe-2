using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;
using API.Repositories;


namespace API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UserController(IUserRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            return Ok(await _repo.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // Modtager en DTO med Username og Password
        [HttpPost("register")]
        public async Task<ActionResult<User>> Create([FromBody] UserRegisterDTO dto)
        {
            var user = new User { Username = dto.Username, IsAdmin = dto.IsAdmin };
            
            // Der sendes både objektet og det rå password til hashing
            var createdUser = await _repo.CreateAsync(user, dto.Password);
            
            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _repo.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}