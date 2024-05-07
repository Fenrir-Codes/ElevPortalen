using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    public class MessageModel
    {
        [Key]
        public int MessageId { get; set; } // Primary key
        public int ReceiverId { get; set; } // ID of the Student or company receiving the message
        public int SendererId { get; set; } // ID of the Student or company sending the message
        public string? SenderName { get; set; } // Name of the Sender
        public string? Subject { get; set; }  // Subject of the message
        public string? Content { get; set; }  // Content of the message
        public DateTime Timestamp { get; set; }  // timestamp
        public bool IsRead { get; set; }  // read or not?
    }
}
