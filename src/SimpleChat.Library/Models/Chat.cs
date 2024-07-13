using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimpleChat.Library.Models
{
    public class Chat
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

        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [Required]
        public User? User { get; set; }
        public List<User> Users { get; set; } = new();
    }
}
