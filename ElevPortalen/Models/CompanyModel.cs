using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    //Lavet af Jozsef
    public class CompanyModel
    {
        public Guid UserId { get; set; }

        [Key]
        public int CompanyId { get; set; }
        //public int LoginId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Region { get; set; }
        public string Email { get; set; }
        public string Link { get; set; }
        public string Preferences { get; set; }
        public string Description { get; set; }
        public int PhoneNumber { get; set; }
        public bool IsHiring { get; set; }
        public bool IsVisible { get; set; }

        //LogoImage is not declared yet we have to find out first how we declare it, as an Image or a string

    }
}
