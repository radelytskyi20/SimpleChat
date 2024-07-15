using SimpleChat.Library.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimpleChat.Library.Models
{
    public class User : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Chat> ChatsCreated { get; set; } = new();

        [JsonIgnore]
        public List<Chat> Chats { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
