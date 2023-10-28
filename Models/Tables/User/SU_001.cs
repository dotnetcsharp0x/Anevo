using System.ComponentModel.DataAnnotations;

namespace Anevo.Models.Tables.User
{
    public class SU_001
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}