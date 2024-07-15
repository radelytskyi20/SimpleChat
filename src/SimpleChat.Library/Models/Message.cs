using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleChat.Library.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime SentTime { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }

        [ForeignKey("ChatId")]
        public Guid ChatId { get; set; }

        public User? User { get; set; }
        public Chat? Chat { get; set; }

        public override string ToString() => $"{Chat?.Name}({Chat?.Id}) | {User?.Name}({User?.Id}): {Content} - {SentTime.ToShortTimeString()}";
    }
}
