using AngularNetCoreAuthDemo.Server.Code.Models;
using AngularNetCoreAuthDemo.Server.Code.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetCoreAuthDemo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountsController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
    }
}
