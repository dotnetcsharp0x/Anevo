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

        public DbSet<Users> Users { get; set; }

        ~ApplicationContext()
        {
        }
    }
}
