using System.Text.Json;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UdvApp.Identity.Data;
using UdvApp.Identity.Models;

namespace UdvApp.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly AuthDbContext _dbContext;

        public AuthController(
            IIdentityServerInteractionService interactionService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AuthDbContext dbContext)
        {
            _interactionService = interactionService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound(request);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

            if (result.Succeeded) 
            {
                return Ok(request.ReturnUrl);
            }

            return BadRequest(request);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest request) 
        {
            var password = "password";   

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var userInfo = new UserInfo
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.PhoneNumber,
                    UniversityName = request.UniversityName,
                    UniversityFaculty = request.UniversityFaculty,
                    UniversitySpeciality = request.UniversitySpeciality,
                    UniversityCourseNumber = request.UniversityCourseNumber,
                    USSCTargets = JsonSerializer.Serialize(request.USSCTargets),
                    USSCTargetDates = request.USSCTargetDates
                };

                await _dbContext.UserInfos.AddAsync(userInfo);
                await _dbContext.SaveChangesAsync();
                //SMTP
                
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(request.ReturnUrl);
            }
            
            return BadRequest(result.Errors);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            return Ok(logoutRequest.PostLogoutRedirectUri);
        }
    }
}
