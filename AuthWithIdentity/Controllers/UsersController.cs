using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthWithIdentity.DLL;
using AuthWithIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthWithIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;

        public UsersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult GetUsers()
        {
            List<string> UserNames = new List<string>();

            foreach (var user in context.Users)
            {
                UserNames.Add(user.UserName);
            }
            return Ok(UserNames);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]SignInModel signInModel)
        {
            var user = await userManager.FindByNameAsync(signInModel.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, signInModel.Password))
            {
                var claims = new Claim[] {
                new Claim (JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsSecretKeyAndShouldNotBeSharedWithAnyOne"));

                var token = new JwtSecurityToken(
                    issuer: "www.test.com",
                    audience: "www.test.com",
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                    expires: DateTime.Now.AddHours(1),
                    claims: claims
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expires = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}