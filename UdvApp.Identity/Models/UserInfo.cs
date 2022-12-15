namespace UdvApp.Identity.Models;

public class UserInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; }
    public string Email { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string UniversityName { get; set; }
    public string UniversityFaculty { get; set; }
    public string UniversitySpeciality { get; set; }
    public int UniversityCourseNumber { get; set; }
    public string USSCTargets { get; set; }
    public string USSCTargetDates { get; set; }
}