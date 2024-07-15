namespace SimpleChat.Library.Requests.Chats
{
    public class RemoveChatRequest
    {
        public Guid ChatId { get; set; }
        public Guid AdminId { get; set; }
    }
}
