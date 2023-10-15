using System.ComponentModel.DataAnnotations;

namespace Anevo.Models.User {
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; } 
        public string Password { get; set; }     
    }
}