using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SimpleChat.Library.Interfaces;

namespace SimpleChat.Library.Models
{
    public class Chat : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime Created { get; set; }

        [ForeignKey("Admin")]
        public Guid AdminId { get; set; }

        [Required]
        public User? AdminUser { get; set; }
        public List<User> Users { get; set; } = new();
        public List<Message> Messages { get; set; } = new();

        public bool IsUserInChat(Guid userId) => Users.Any(u => u.Id == userId);
    }
}
