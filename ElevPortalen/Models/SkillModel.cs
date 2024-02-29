using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElevPortalen.Models
{
    public class SkillModel
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? SkillName { get; set; }

        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }
    }
}
