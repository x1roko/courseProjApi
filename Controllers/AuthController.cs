using courseProjAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace courseProjAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BrosShopDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BrosShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            // Проверка на существование пользователя
            if (_context.BrosShopUsers.Any(u => u.BrosShopUsername == userDto.Username))
            {
                return BadRequest("Пользователь с таким именем уже существует.");
            }

            // Создание нового пользователя
            var user = new BrosShopUser
            {
                BrosShopUsername = userDto.Username,
                BrosShopPassword = userDto.Password,
                BrosShopPhoneNumber = userDto.PhoneNumber
            };

            _context.BrosShopUsers.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Пользователь успешно зарегистрирован.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = _context.BrosShopUsers.SingleOrDefault(u => u.BrosShopUsername == userDto.Username);
            if (user == null || userDto.Password != user.BrosShopPassword)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(BrosShopUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.BrosShopUsername),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"])); // Используйте тот же ключ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(7200),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
