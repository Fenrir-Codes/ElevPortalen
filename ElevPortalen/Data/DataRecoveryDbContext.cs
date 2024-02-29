using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Data
{
    public class DataRecoveryDbContext : DbContext
    {
        public DataRecoveryDbContext(DbContextOptions<DataRecoveryDbContext> options) : base(options){}

        public DbSet<StudentRecoveryModel> StudentDataRecovery { get; set; }
        public DbSet<CompanyRecoveryModel> CompanyDataRecovery { get; set; }
    }
}
