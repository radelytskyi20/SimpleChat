using Microsoft.AspNetCore.SignalR;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Models;

namespace SimpleChat.Library.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IRepo<Chat> _chatsRepo;
        private readonly IRepo<User> _usersRepo;

        public ChatHub(IRepo<Chat> chatsRepo, IRepo<User> usersRepo)
        {
            _chatsRepo = chatsRepo;
            _usersRepo = usersRepo;
        }

        public async Task SendMessage(Guid chatId, Guid userId, string message)
        {
            try
            {
                var (user, chat) = await GetUserAndChat(userId, chatId);
                if (user == null || chat == null) { return; }

                if (!chat.IsUserInChat(user.Id))
                {
                    await Clients.Caller.SendAsync("onError", $"User with id {userId} is not a member of chat with id {chatId}");
                    return;
                }

                var chatMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    Content = message,
                    SentTime = DateTime.UtcNow,
                    User = user,
                    Chat = chat
                };

                //toDo: add message to Db

                await JoinChat(chat.Id, user);
                await Clients.Group(chatId.ToString()).SendAsync("onMessageReceived", chatMessage.ToString());
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", $"An error occurred while sending message: {ex.Message}");
            }
        }

        public async Task JoinChat(Guid userId, Guid chatId)
        {
            try
            {
                var (user, chat) = await GetUserAndChat(userId, chatId);
                if (user == null || chat == null) { return; }

                if (!chat.Users.Any(u => u.Id == user.Id))
                {
                    chat.Users.Add(user);
                    await _chatsRepo.UpdateAsync(chat);
                }

                await JoinChat(chat.Id, user);
                await Clients.Caller.SendAsync("onJoinedChat", $"{user.Name}({user.Id}): connected to {chat.Name}({chat.Id}) chat");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", $"An error occurred while joining to chat: {ex.Message}");
            }
        }

        public async Task LeaveChat(Guid userId, Guid chatId)
        {
            var (user, chat) = await GetUserAndChat(userId, chatId);
            if (user == null || chat == null) { return; }

            if (!chat.IsUserInChat(user.Id))
            {
                await Clients.Caller.SendAsync("onError", $"User with id {userId} is not a member of chat with id {chatId}");
                return;
            }

            if (chat.UserId == user.Id)
            {
                await Clients.Caller.SendAsync("onError", $"User with id {userId} is an admin of chat with id {chatId}. Admin cannot leave chat");
                return;
            }

            chat.Users.Remove(user);
            await _chatsRepo.UpdateAsync(chat);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
            await Clients.Caller.SendAsync("onLeftChat", $"{user.Name}({user.Id}): disconnected from {chat.Name}({chat.Id}) chat");
        }

        private async Task JoinChat(Guid chatId, User user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await Clients.OthersInGroup(chatId.ToString()).SendAsync("addUser", user);
        }
        private async Task<(User? user, Chat? chat)> GetUserAndChat(Guid userId, Guid chatId)
        {
            var chat = await _chatsRepo.GetOneAsync(chatId);
            if (chat == null)
            {
                await Clients.Caller.SendAsync("onError", $"Could not find chat with given id: {chatId}");
                return (null, null);
            }

            var user = await _usersRepo.GetOneAsync(userId);
            if (user == null)
            {
                await Clients.Caller.SendAsync("onError", $"Could not find user with given id: {userId}");
                return (null, null);
            }

            return (user, chat);
        }
    }
}
