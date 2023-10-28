using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Anevo.Models.Tables.Groups
{
    public class SG_001
    {
        public int SG001_Id { get; set; }
        public int SU001_Id_User { get; set; }
        public int SG001_GroupNr { get; set; }
    }
}
