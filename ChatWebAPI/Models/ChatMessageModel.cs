using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebAPI.Models
{
    public class ChatMessageModel
    {
        public string Id { get; set; }

        public string MessageText { get; set; }

        public string MessageSentimet {  get; set; }
        public DateTime Date { get; set; }


        [ForeignKey("Chat")]
        public int ChatId {  get; set; }
        public Chat Chat { get; set; }
    }
}
