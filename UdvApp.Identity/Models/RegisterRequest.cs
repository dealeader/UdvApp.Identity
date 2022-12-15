using System.ComponentModel.DataAnnotations;

namespace UdvApp.Identity.Models
{
    public class RegisterRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string ReturnUrl { get; set; }

    }
}
