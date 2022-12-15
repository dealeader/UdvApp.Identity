using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(
            IIdentityServerInteractionService interactionService,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _interactionService = interactionService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                FirstName = "testFirstName",
                LastName = "testLastName",
                Patronymic = "testPatronymic"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //SMTP
                await _signInManager.SignInAsync(user, isPersistent: false);
                request.ReturnUrl = "https://localhost:7079/";
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
