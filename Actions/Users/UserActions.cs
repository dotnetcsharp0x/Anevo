using Anevo.Data;
using Anevo.Enums.SU0010;
using Anevo.Models.Tables.Groups;
using Anevo.Models.Tables.User;
using Microsoft.EntityFrameworkCore;

namespace Anevo.Actions.Users
{
    public class UserActions
    {
        private readonly ApplicationContext _context;
        public UserActions(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<SU_001> GetUserByEmail(string email)
        {
            return await _context.SU_001.Where(x => x.Email == email).FirstOrDefaultAsync();
        }
        public async Task<List<SU_001>> GetUsers()
        {
            return await _context.SU_001.ToListAsync();
        }

        public async Task CreateUser(SU_001 user)
        {
            await _context.SU_001.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
