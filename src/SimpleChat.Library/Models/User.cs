using SimpleChat.Library.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public List<Chat> ChatsCreated { get; set; } = new();
        public List<Chat> Chats { get; set; } = new();
    }
}
