using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatWebAPI.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ChatName { get; set; }

        public ICollection<RegistrationModel> Users { get; set; } = [];

        public int CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public RegistrationModel Creator { get; set; }

        public ICollection<ChatMessageModel> ChatDetails { get; set; }
    }
}
