using App.Data.Data;
using App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        AppDbContext _context;
        public AuthController()
        {
             _context = new AppDbContext();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string usn, string pass)
        {
            var acc = _context.Accounts.FirstOrDefault(x => x.Username == usn && x.Password == pass);

            if (acc == null)
            {
                return Unauthorized();
            }
            else
            {
                var token = GenerateJwtToken(acc.Username);
                return Ok(new { token });
            }
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("daylakhoabimatcuagiaphieuchua1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://asmgd2cs5.auth0.com/",
                audience: "https://api.asmgd2cs5.com/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
