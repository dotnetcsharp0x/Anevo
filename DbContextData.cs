using Microsoft.EntityFrameworkCore;
using Anevo.Models.User;

namespace Anevo
{
    public class DbContextData : DbContext 
    {
        public DbContextData(DbContextOptions<DbContextData> option):base(option)
        {

        }
        public virtual DbSet<Users> Users { get; set; }
    }
}