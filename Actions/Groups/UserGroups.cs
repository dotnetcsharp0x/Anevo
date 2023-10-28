using Anevo.Data;
using Anevo.Enums.SU0010;
using Anevo.Models.Tables.Groups;
using Anevo.Models.Tables.User;
using Microsoft.EntityFrameworkCore;

namespace Anevo.Actions.Groups
{
    public class UserGroups
    {
        private readonly ApplicationContext _context;
        public UserGroups(ApplicationContext context) {
            _context = context;
        }
        public async Task AddUserToGroup(SU_001 user, SU010_Types group_nr)
        {
            await _context.SG_001.AddAsync(new SG_001 { SU001_Id_User = user.Id, SG001_GroupNr = (int)group_nr });
            await _context.SaveChangesAsync();
        }

        public async Task<SG_001> GetUserInGroup(int id_user)
        {
            return await _context.SG_001.Where(x => x.SU001_Id_User == id_user).FirstOrDefaultAsync();
        }

        public async Task<SG_010> GetGroup(int group_nr)
        {
            return await _context.SG_010.Where(x => x.SG010_Group_Nr == group_nr).FirstOrDefaultAsync();
        }
    }
}
