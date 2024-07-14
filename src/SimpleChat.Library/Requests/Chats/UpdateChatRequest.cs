namespace SimpleChat.Library.Requests.Chats
{
    public class UpdateChatRequest
    {
        public Guid ChatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public Guid AdminUserId { get; set; }
    }
}
