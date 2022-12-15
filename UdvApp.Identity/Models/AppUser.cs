using Microsoft.AspNetCore.Identity;

namespace UdvApp.Identity.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        
    }
}
