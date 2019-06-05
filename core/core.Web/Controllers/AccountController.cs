using core.Domain.Entities.User;
using core.Services.Services.Auth;
using core.Services.Services.Text;
using core.Services.Services.User;
using core.Web.ViewModels.Account.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace core.Web.Controllers
{
    [Authorize]
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IUserService userService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterVM vm)
        {
            if (ModelState.IsValid)
            {
                var username = vm.Username.ToLower();
                var validUsername = await _userService.IsUsernameValid(username);

                if (validUsername)
                {
                    var user = new ApplicationUser
                    {
                        Name = vm.Name,
                        UserName = username,
                        Email = vm.Email.ToLower()
                    };

                    var result = await _userManager.CreateAsync(user, vm.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                        //await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        if (vm.GrantType == "password")
                        {
                            var tokenResponse = await _tokenService.GetTokensAsync(user, vm.ClientId, vm.Scope, false);

                            if (tokenResponse != null)
                            {
                                return Ok(tokenResponse);
                            }
                        }
                    }
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.GrantType == "password")
                {
                    var user = await _userManager.FindByEmailAsync(vm.Email);

                    if (user != null)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(user, vm.Password, true);

                        if (result.Succeeded)
                        {
                            var tokenResponse = await _tokenService.GetTokensAsync(user, vm.ClientId, vm.Scope, vm.RememberMe);

                            if (tokenResponse != null)
                            {
                                return Ok(tokenResponse);
                            }
                        }
                    }
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("renew")]
        public async Task<IActionResult> RenewTokens([FromBody]RenewTokenVM vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.GrantType == "refresh_token")
                {
                    Guid refreshToken;

                    if (Guid.TryParseExact(vm.RefreshToken, "n", out refreshToken))
                    {
                        var tokenResponse = await _tokenService.RenewTokensAsync(refreshToken, vm.ClientId, vm.Scope);

                        if (tokenResponse != null)
                        {
                            return Ok(tokenResponse);
                        }
                    }
                }
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("revoke")]
        public IActionResult RevokeToken([FromBody]RevokeTokenVM vm)
        {
            if (ModelState.IsValid)
            {
                Guid refreshToken;

                if (Guid.TryParseExact(vm.RefreshToken, "n", out refreshToken))
                {
                    _tokenService.RevokeTokenAsync(refreshToken); //don't want to wait
                }
            }

            return Ok(); //don't throw an error here since logging out
        }

        [AllowAnonymous]
        [HttpGet("username")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var model = await _userService.IsUsernameValid(username.ToLower());

            return Ok(model);
        }
    }
}