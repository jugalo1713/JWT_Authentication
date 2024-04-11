using JWT_Authentication.Models;
using JWT_Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private List<User> _users = new List<User>
        {
            new User { UserName = "Admin", Password = "Password", DisplayName = "Julian", Email = "julianlondono@outlook.com" },
            new User { UserName = "User2", Password = "Password2", DisplayName = "Julian2", Email = "julianlondono@outlook.com2"},
            new User { UserName = "User3", Password = "Password3", DisplayName = "Julian3", Email = "julianlondono@outlook.com3"}
        };
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(User user)
        {
            var token = _userService.Login(user);
            if (token == null || token == string.Empty)
            {
                return BadRequest(new { message = "UserName or Password is incorrect" });
            }
            return Ok(token);
        }

        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAll()
        {
            var reques = HttpContext.Request;
            var context = HttpContext;


            return context.User.Identity.IsAuthenticated? Ok(_users) : Unauthorized();
        }
    }
}
