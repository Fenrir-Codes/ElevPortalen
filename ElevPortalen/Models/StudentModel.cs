using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    //Lavet af Jozsef
    public class StudentModel
    {
        public Guid UserId { get; set; }

        [Key]
        public int StudentId { get; set; }
        public string? Title { get; set; } // jeg tror vi skal ikke have title bruger vi specialisering i stedet for
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Description{ get; set; }
        public string? profileImage { get; set; }
        public string? Speciality { get; set; }
        public int PhoneNumber { get; set; }
        public string? FacebookLink { get; set; }
        public string? LinkedInLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? GitHubLink { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        public virtual SkillModel? Skills { get; set; }

    }
}
