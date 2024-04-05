using ElevPortalen.Models;
using Microsoft.EntityFrameworkCore;

namespace ElevPortalen.Data
{
    public class JobOfferDbContext : DbContext
    {
        public JobOfferDbContext(DbContextOptions<JobOfferDbContext> options) : base(options) { }

        public DbSet<JobOfferModel> JobOfferDataBase { get; set; }

    }
}
