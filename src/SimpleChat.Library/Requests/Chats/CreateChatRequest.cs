using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Library.Requests.Chats
{
    public class CreateChatRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
    }
}
