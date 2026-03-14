using API.partonair_v01.Token;
using BLL.partonair_v01.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels.partonair_v01.DTOS;

namespace API.partonair_v01.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SigninDefaultController(TokenService tokenService, IUserService userService) : ControllerBase
    {

        private readonly TokenService _tokenService = tokenService;
        private readonly IUserService _userService = userService;


        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO log)
        {
            var result = await _userService.LogTest(log.Email, log.Password);

            if (result is not null)
            {
                string token = _tokenService.CreateToken(result);
                return Ok(new LoginDefaultResponse(token));
            }
            return BadRequest("Les identifiants ne sont pas corrects");
        }
    }
}
