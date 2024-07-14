using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Models;
using SimpleChat.Library.Requests.Chats;

namespace SimpleChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ChatsController : ControllerBase
    {
        private readonly IRepo<Chat> _chatsRepo;
        private readonly IRepo<User> _usersRepo;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(IRepo<Chat> chatsRepo, IRepo<User> usersRepo, ILogger<ChatsController> logger)
        {
            _chatsRepo = chatsRepo;
            _usersRepo = usersRepo;
            _logger = logger;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromBody] CreateChatRequest request)
        {
            try
            {
                var adminUser = await _usersRepo.GetOneAsync(request.UserId);
                if (adminUser == null)
                {
                    return BadRequest($"Unable to create new chat. User with given id {request.UserId} does not exist");
                }

                var chat = new Chat() { Id = Guid.NewGuid(), Name = request.Name, Created = DateTime.UtcNow, User = adminUser };
                chat.Users.Add(adminUser);

                var chatId = await _chatsRepo.AddAsync(chat);
                return Ok(chatId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete(RepoActions.Remove)]
        public async Task<IActionResult> Remove([FromQuery] Guid id)
        {
            //Todo: check if user is admin of chat
            try
            {
                await _chatsRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut(RepoActions.Update)]
        public async Task<IActionResult> Update([FromBody] UpdateChatRequest request)
        {
            try
            {
                var chatToUpdate = await _chatsRepo.GetOneAsync(request.ChatId);
                if (chatToUpdate == null)
                {
                    return BadRequest($"Chat with given id {request.ChatId} was not found");
                }

                if (request.AdminUserId != chatToUpdate.UserId)
                {
                    return BadRequest($"User with given id {request.AdminUserId} is not an admin of chat with id {request.ChatId}");
                }

                var adminUser = await _usersRepo.GetOneAsync(request.UserId ?? chatToUpdate.UserId);
                chatToUpdate.Name = request.Name;
                chatToUpdate.User = adminUser;

                await _chatsRepo.UpdateAsync(chatToUpdate);
                return Ok(chatToUpdate);
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
                var chats = await _chatsRepo.GetAllAsync();
                return Ok(chats);
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
                var chat = await _chatsRepo.GetOneAsync(id);
                if (chat == null)
                {
                    return NotFound();
                }

                return Ok(chat);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
