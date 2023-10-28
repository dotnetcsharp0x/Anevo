using Anevo.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Anevo.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SU001>().HasKey(u => new { u.SU001_Id });
            modelBuilder.Entity<SU010>().HasKey(u => new { u.SU010_Id });
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<SU001> SU001 { get; set; }

        public DbSet<SU010> SU010 { get; set; }

        ~ApplicationContext()
        {
        }
    }
}
