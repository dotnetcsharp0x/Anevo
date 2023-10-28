using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anevo.Models.User
{
    public class SU001
    {
        public int SU001_Id { get; set; }
        public int SU001_Id_User { get; set; }
        public int SU001_GroupNr {  get; set; }
    }
}
