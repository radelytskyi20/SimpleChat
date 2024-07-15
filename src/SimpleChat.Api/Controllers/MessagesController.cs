using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Models;

namespace SimpleChat.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class MessagesController : ControllerBase
    {
        private readonly IRepo<Message> _messagesRepo;
        private readonly IRepo<Chat> _chatsRepo;
        private readonly IRepo<User> _usersRepo;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IRepo<Message> messagesRepo, IRepo<Chat> chatsRepo, IRepo<User> usersRepo, ILogger<MessagesController> logger)
        {
            _messagesRepo = messagesRepo;
            _chatsRepo = chatsRepo;
            _usersRepo = usersRepo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOne([FromQuery] Guid id)
        {
            try
            {
                var message = await _messagesRepo.GetOneAsync(id);
                if (message == null)
                {
                    return BadRequest($"Message with id {id} was not found");
                }

                return Ok(message);
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
                var messages = await _messagesRepo.GetAllAsync();
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet($"{RepoActions.GetAll}/chat")]
        public async Task<IActionResult> GetAllChat([FromQuery] Guid id)
        {
            try
            {
                var chat = await _chatsRepo.GetOneAsync(id);
                if (chat == null)
                {
                    return BadRequest($"Chat with id {id} was not found");
                }

                var chatMessages = chat.Messages
                    .OrderByDescending(m => m.SentTime)
                    .Select(m => m.ToString())
                    .ToList();
                
                return Ok(chatMessages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet($"{RepoActions.GetAll}/user")]
        public async Task<IActionResult> GetAllUser([FromQuery] Guid id)
        {
            try
            {
                var user = await _usersRepo.GetOneAsync(id);
                if (user == null)
                {
                    return BadRequest($"User with id {id} was not found");
                }

                var userMessages = user.Messages
                    .OrderByDescending(m => m.SentTime)
                    .Select(m => m.ToString())
                    .ToList();

                return Ok(userMessages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
