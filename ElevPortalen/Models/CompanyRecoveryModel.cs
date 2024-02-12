using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    public class CompanyRecoveryModel
    {
        public Guid UserId { get; set; }

        [Key]
        public int HistoryId { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? Region { get; set; }
        public string? Email { get; set; }
        public string? Link { get; set; }
        public string? Preferences { get; set; }
        public string? Description { get; set; }
        public string? profileImage { get; set; }
        public int PhoneNumber { get; set; }
        public bool IsHiring { get; set; }
        public bool IsVisible { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime RecoveryCreatedDate{ get; set; }
    }
}
