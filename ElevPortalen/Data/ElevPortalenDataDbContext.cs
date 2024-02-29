using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Data
{   //Lavet af Jozsef
    public class ElevPortalenDataDbContext : DbContext
    {
        public ElevPortalenDataDbContext(DbContextOptions<ElevPortalenDataDbContext> options) : base(options){}

        public DbSet<StudentModel> Student { get; set; }
        public DbSet<CompanyModel> Company { get; set; }
        public DbSet<SkillModel> StudentSkills { get; set; }
    }
}
