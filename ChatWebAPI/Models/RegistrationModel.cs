using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebAPI.Models
{
    public class RegistrationModel /*: IdentityUser*/ 
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }   
        
        public string Password { get; set; }


        public virtual ICollection<Chat> Chats { get; set; } = [];
        public virtual ICollection<Chat> CreatedChats { get; set; } = [];   

    }
}
