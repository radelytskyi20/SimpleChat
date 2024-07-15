using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Logging;
using SimpleChat.Library.Models;
using SimpleChat.Library.Requests.Users;

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
        public async Task<IActionResult> Add([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                var userId = await _repo.AddAsync(user);
                return Ok(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(user.Id)}: {user.Id}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpDelete(RepoActions.Remove)]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            try
            {
                var user = await _repo.GetOneAsync(id);
                if (user == null)
                {
                    return BadRequest($"User with id {id} was not found");
                }

                if (user.ChatsCreated.Count != 0)
                {
                    return BadRequest($"User with id {id} cannot be deleted because they are the creator of one or more chats");
                }

                await _repo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"{nameof(id)}: {id}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
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
                    return BadRequest($"User with id {request.Id} was not found");
                }

                userToBeUpdated.Name = request.Name;
                await _repo.UpdateAsync(userToBeUpdated);
                return Ok(userToBeUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(request.Id)}: {request.Id}, {nameof(request.Name)}: {request.Name}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
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
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                );
                
                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet(UsersRoutes.GetAllByName)]
        public async Task<IActionResult> GetAll([FromRoute] string name)
        {
            try
            {
                var users = await _repo.GetAllAsync();
                var filteredUsers = users.Where(u => u.Name.Contains(name)).ToList();
                return Ok(filteredUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.Message)
                    .WithOperation(UsersRoutes.GetAllByName)
                    .WithParametres($"{nameof(name)}: {name}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
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
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(UsersController))
                    .WithMethod(nameof(GetOne))
                    .WithComment(ex.Message)
                    .WithOperation("Get")
                    .WithParametres($"{nameof(id)}: {id}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }  
        }
    }  
}
