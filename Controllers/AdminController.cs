using courseProjApi.Models;
using courseProjAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace courseProjApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly BrosShopDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(BrosShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminDTO adminDTO)
        {
            var admin = _context.Administrators.SingleOrDefault(u => u.BrosShopLogin == adminDTO.Login);
            if (admin == null || adminDTO.Password != admin.BrosShopPassword)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(admin);
            return Ok(new { Token = token });
        }


        private string GenerateJwtToken(Administrator admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.BrosShopLogin),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"])); // Используйте тот же ключ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(720),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}