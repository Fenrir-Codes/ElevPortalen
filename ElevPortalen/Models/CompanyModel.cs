using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    //Lavet af Jozsef
    public class CompanyModel
    {
        public Guid UserId { get; set; }

        [Key]
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
        //public string? FacebookLink { get; set; }
        //public string? LinkedInLink { get; set; }
        //public string? InstagramLink { get; set; }
        //public string? GitHubLink { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
