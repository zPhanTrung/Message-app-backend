using Message_app_backend.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Message_app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public IConfiguration configuration;
        private readonly AppDb context;

        public AuthenticationController(IConfiguration config, AppDb context)
        {
            configuration = config;
            this.context = context;
        }

        [HttpPost]
        [Route("/auth")]
        public async Task<IActionResult> Post(UserInfo userData)
        {
            if (userData != null && userData.UserName != null && userData.Password != null)
            {
                var user = await GetUser(userData.UserName, userData.Password);

                if (user?.UserName != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("UserName", user.UserName),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        configuration["Jwt:Issuer"],
                        configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token).ToString() });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<UserInfo?> GetUser(string userName, string password)
        {
            return await context.UserInfos.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
        }
    }

}
