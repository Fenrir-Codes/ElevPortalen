using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElevPortalen.Models
{
    public class SkillModel
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }

        // Programming languages
        public bool CSharp { get; set; }
        public bool Java { get; set; }
        public bool DotNet { get; set; }
        public bool Typescript { get; set; }
        public bool Python { get; set; }
        public bool PHP { get; set; }
        public bool CPlusPlus { get; set; }
        public bool C { get; set; }

        // Web development technologies
        public bool Bootstrap { get; set; }
        public bool Blazor { get; set; }
        public bool JavaScript { get; set; }
        public bool HTML { get; set; }
        public bool CSS { get; set; }

        // Database technologies
        public bool SQL { get; set; }

        // Other skills
        public bool OfficePack { get; set; }
        public bool CloudComputing { get; set; }
        public bool VersionControl { get; set; }
        public bool ProblemSolving { get; set; }
        public bool Communikation { get; set; }
        public bool TeamWorking { get; set; }
        public bool WillingToLearn { get; set; }
        public bool NetWork { get; set; }


        [ForeignKey("StudentId")]
        public virtual StudentModel? Student { get; set; }
    }
}
