using AngularNetCoreAuthDemo.Server.Code.Models;
using AngularNetCoreAuthDemo.Server.Code.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AngularNetCoreAuthDemo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        //api/accounts/register
        public async Task<ActionResult<string>> Register(RegisterViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser user = new AppUser
            {
                Email = viewModel.Email,
                FullName = viewModel.FullName,
                UserName = viewModel.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);

            if(!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new AuthResponseViewModel
            {
                Result = true, 
                Message = "Accout created"
            });
        }

        [HttpPost("login")]
        //api/accounts/login
        public async Task<ActionResult<AuthResponseViewModel>> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser? user = await _userManager.FindByEmailAsync(viewModel.Email);

            if (user == null)
            {
                return Unauthorized(new AuthResponseViewModel
                {
                    Result = false,
                    Message = "Invalid username and password combination",
                    Token = ""
                });
            }

            bool result = await _userManager.CheckPasswordAsync(user, viewModel.Password);

            if (!result)
            {
                return Unauthorized(new AuthResponseViewModel
                {
                    Result = false,
                    Message = "Invalid username and password combination"
                });
            }

            string token = CreateToken(user);

            return Ok(new AuthResponseViewModel
            {
                Token = token,
                Result = true,
                Message = "Login Successful"
            });
        }

        private string CreateToken(AppUser user)
        {
            IConfigurationSection? authSettings = _configuration.GetSection("AuthSettings");

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] securityKey = Encoding.ASCII.GetBytes(authSettings.GetSection("securityKey").Value!);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Aud, authSettings.GetSection("validAudience").Value!),
                new Claim(JwtRegisteredClaimNames.Iss, authSettings.GetSection("validIssuer").Value!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? "")
            };

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(securityKey),
                    SecurityAlgorithms.HmacSha256
                )
            };

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
