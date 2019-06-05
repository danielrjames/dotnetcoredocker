using core.Domain.Entities.User;
using core.Services.Services.User;
using core.Web.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace core.Web.Controllers
{
    [Authorize]
    [Route("response")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public ResponseController(UserManager<ApplicationUser> userManager,IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsers();

            var model = new ResponsesVM(users.ToList());

            return Ok(model);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitResponse([FromBody] ResponseSubmissionVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    if (!user.Submitted)
                    {
                        user.Response = vm.Response;
                        user.Submitted = true;

                        await _userManager.UpdateAsync(user);
                    }

                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}