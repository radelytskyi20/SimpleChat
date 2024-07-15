namespace SimpleChat.Library.Requests.Chats
{
    public class UpdateChatRequest
    {
        public Guid ChatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? NewAdminId { get; set; }
        public Guid AdminId { get; set; }
    }
}
