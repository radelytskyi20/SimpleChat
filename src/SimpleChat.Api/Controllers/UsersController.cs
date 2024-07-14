using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Models;
using SimpleChat.Library.Requests;

namespace SimpleChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IRepo<User> _repo;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IRepo<User> repo, ILogger<UsersController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest();
                }

                var user = new User() { Id = Guid.NewGuid(), Name = name };
                var userId = await _repo.AddAsync(user);
                return Ok(userId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete(RepoActions.Remove)]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut(RepoActions.Update)]
        public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
        {
            try
            {
                var userToBeUpdated = await _repo.GetOneAsync(request.Id);
                if (userToBeUpdated == null)
                {
                    return BadRequest($"User with given id {request.Id} was not found");
                }

                userToBeUpdated.Name = request.Name;
                await _repo.UpdateAsync(userToBeUpdated);
                return Ok(userToBeUpdated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet(RepoActions.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _repo.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOne([FromQuery] Guid id)
        {
            try
            {
                var user = await _repo.GetOneAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }  
        }
    }  
}
