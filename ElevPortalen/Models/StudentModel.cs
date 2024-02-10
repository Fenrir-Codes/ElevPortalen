using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    //Lavet af Jozsef
    public class StudentModel
    {
        public Guid UserId { get; set; }

        [Key]
        public int StudentId { get; set; }
        public string? Title { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Description{ get; set; }
        public string? profileImage { get; set; }
        public string? Speciality { get; set; }
        public int PhoneNumber { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
