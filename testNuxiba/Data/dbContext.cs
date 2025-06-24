using Microsoft.EntityFrameworkCore;
using testNuxiba.Models;

namespace testNuxiba.Data
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options) { }
       
        public DbSet<User> ccUsers { get; set; }
        public DbSet<Login> ccLogLogin { get; set; }
        public DbSet<Area> ccRIACat_Areas { get; set; }
    }
}
