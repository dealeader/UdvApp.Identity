using System.Text;
using System.Text.Json;
using IdentityServer4.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using PasswordGenerator;
using UdvApp.Identity.Data;
using UdvApp.Identity.Models;
using UdvApp.Identity.Service.EmailService;

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
        private readonly IEmailService _emailService;

        public AuthController(
            IIdentityServerInteractionService interactionService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AuthDbContext dbContext,
            IEmailService emailService)
        {
            _interactionService = interactionService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _emailService = emailService;
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
            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
            };

            var password = new Password(12)
                .IncludeLowercase()
                .IncludeNumeric()
                .IncludeUppercase()
                .IncludeSpecial()
                .Next();

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

                _emailService.SendEmail(new Email
                {
                    To = user.Email,
                    Body = "Пароль для входа в UDV Summer School ",
                    Subject = "Добро пожаловать в UDV Summer School"
                }, password);

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
