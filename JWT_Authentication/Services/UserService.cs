using JWT_Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private List <User> _users = new List<User>
        {
            new User { UserName = "Admin", Password = "Password", DisplayName = "Julian", Email = "julianlondono@outlook.com" },
            new User { UserName = "User2", Password = "Password2", DisplayName = "Julian2", Email = "julianlondono@outlook.com2"},
            new User { UserName = "User3", Password = "Password3", DisplayName = "Julian3", Email = "julianlondono@outlook.com3"}
        };
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Login(User user)
        {
            var loginUser = _users.SingleOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);

            if(loginUser == null)
            {
                return "";
            }

            var tokenHandler = new JwtSecurityTokenHandler();   
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, loginUser.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud,_configuration["Jwt:Audience"]),
                    new Claim(JwtRegisteredClaimNames.Iss,_configuration["Jwt:Issuer"]),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("DisplayName", user.DisplayName),
                    new Claim("UserName", user.UserName),
                    new Claim("Email", user.Email)

                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
