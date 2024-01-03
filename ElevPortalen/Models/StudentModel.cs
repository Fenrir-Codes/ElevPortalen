using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    //Lavet af Jozsef
    public class StudentModel
    {
        public Guid UserId { get; set; }

        [Key]
        public int StudentId { get; set; }
        //public int LoginId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Description{ get; set; }
        public string Speciality { get; set; }
        public int PhoneNumber { get; set; }

        //Image is not declared yet we have to find out first how we declare it, as an Image or a string
    }
}
