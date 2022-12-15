using System.ComponentModel.DataAnnotations;

namespace UdvApp.Identity.Models
{
    public class RegisterRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string UniversityName { get; set; }
        public string UniversityFaculty { get; set; }
        public string UniversitySpeciality { get; set; }
        public int UniversityCourseNumber { get; set; }
        public List<USSCTarget> USSCTargets { get; set; }
        public string USSCTargetDates { get; set; }
        public string ReturnUrl { get; set; }

    }
}
