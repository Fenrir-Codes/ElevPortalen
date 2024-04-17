using System.ComponentModel.DataAnnotations;

namespace ElevPortalen.Models
{
    public class JobOfferModel
    {
        [Key]
        public int JobOfferId { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public int? PhoneNumber { get; set; }
        public string? Title { get; set; }
        public string? JobAddress { get; set; }
        public string? JobLink { get; set; }
        public string? JobDetails { get; set; }
        public int NumberOfPositionsAvailable { get; set; }
        public string? Speciality { get; set;}
        public DateTime DateOfPublish { get; set; }
        public DateTime Deadline { get; set; }
    }
}
