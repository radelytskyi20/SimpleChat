using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Hubs;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Logging;
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
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger<ChatsController> _logger;

        public ChatsController(IRepo<Chat> chatsRepo, IRepo<User> usersRepo, IHubContext<ChatHub> chatHub, ILogger<ChatsController> logger)
        {
            _chatsRepo = chatsRepo;
            _usersRepo = usersRepo;
            _chatHub = chatHub;
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

                var chat = new Chat() { Id = Guid.NewGuid(), Name = request.Name, Created = DateTime.UtcNow, AdminUser = adminUser };
                chat.Users.Add(adminUser);

                var chatId = await _chatsRepo.AddAsync(chat);
                return Ok(chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(request.Name)}: {request.Name}, {nameof(request.UserId)}: {request.UserId}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpDelete(RepoActions.Remove)]
        public async Task<IActionResult> Remove([FromBody] RemoveChatRequest request)
        {
            try
            {
                var chat = await _chatsRepo.GetOneAsync(request.ChatId);
                if (chat == null)
                {
                    return BadRequest($"Chat with given id {request.ChatId} was not found");
                }

                if (chat.AdminId != request.AdminId)
                {
                    return BadRequest($"User with given id {request.AdminId} is not an admin of chat with id {request.ChatId}");
                }

                foreach (var user in chat.Users)
                    await _chatHub.Clients.Group(chat.Id.ToString()).SendAsync("onLeftChat", $"{user.Name}({user.Id}): disconnected from {chat.Name}({chat.Id}) chat");

                await _chatHub.Clients.Group(chat.Id.ToString()).SendAsync("onChatDeleted", $"{chat.Name}({chat.Id}): chat has been removed");
                await _chatsRepo.DeleteAsync(request.ChatId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Remove)
                    .WithParametres($"{nameof(request.ChatId)}: {request.ChatId}, {nameof(request.AdminId)}: {request.AdminId}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
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

                if (request.AdminId != chatToUpdate.AdminId)
                {
                    return BadRequest($"User with given id {request.AdminId} is not an admin of chat with id {request.ChatId}");
                }
                var adminUser = await _usersRepo.GetOneAsync(request.NewAdminId ?? chatToUpdate.AdminId);

                var oldChatName = chatToUpdate.Name;
                var oldAdminName = chatToUpdate.AdminUser?.Name;
                var oldAdminId = chatToUpdate.AdminUser?.Id;

                chatToUpdate.Name = request.Name;
                chatToUpdate.AdminUser = adminUser;

                await _chatsRepo.UpdateAsync(chatToUpdate);
                await _chatHub.Clients.Group(chatToUpdate.Id.ToString()).SendAsync("onChatUpdated", $"{oldChatName}({chatToUpdate.Id}) - {oldAdminName}({oldAdminId}) " +
                    $"-> {chatToUpdate.Name}({chatToUpdate.Id}) - {chatToUpdate?.AdminUser?.Name}({chatToUpdate?.AdminUser?.Id}) chat has been updated");

                return Ok(chatToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.Update)
                    .WithParametres($"{nameof(request.ChatId)}: {request.ChatId}, {nameof(request.Name)}: {request.Name}, {nameof(request.NewAdminId)}: {request.NewAdminId}," +
                    $"{nameof(request.AdminId)}: {request.AdminId}")
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
                var chats = await _chatsRepo.GetAllAsync();
                return Ok(chats);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.Message)
                    .WithOperation(RepoActions.GetAll)
                    .WithParametres(LoggingConstants.NoParameters)
                    .ToString()
                );
                
                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }
        [HttpGet(ChatsRoutes.GetAllByUser)]
        public async Task<IActionResult> GetAll([FromQuery] Guid id)
        {
            try
            {
                var user = await _usersRepo.GetOneAsync(id);
                if (user == null)
                {
                    return BadRequest($"User with id {id} was not found");
                }

                var chats = await _chatsRepo.GetAllAsync();
                var userChats = chats.Where(c => c.IsUserInChat(id)).ToList();
                return Ok(userChats);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.Message)
                    .WithOperation(ChatsRoutes.GetAllByUser)
                    .WithParametres($"{nameof(id)}: {id}")
                    .ToString()
                );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet(ChatsRoutes.GetAllByName)]
        public async Task<IActionResult> GetAll([FromRoute] string name)
        {
            try
            {
                var chats = await _chatsRepo.GetAllAsync();
                var filteredChats = chats.Where(c => c.Name.Contains(name)).ToList();
                return Ok(filteredChats);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.Message)
                    .WithOperation(ChatsRoutes.GetAllByName)
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
                var chat = await _chatsRepo.GetOneAsync(id);
                if (chat == null)
                {
                    return NotFound();
                }

                return Ok(chat);
            }
            catch (Exception ex)
            {
                _logger.LogError(new LogEntry()
                    .WithClass(nameof(ChatsController))
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
