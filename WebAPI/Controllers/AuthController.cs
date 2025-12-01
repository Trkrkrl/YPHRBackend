using Business.Abstract;
using Business.Constants;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {//
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("loginwithmail")]
        public async Task< ActionResult >Login(UserMailLoginDto userMailLoginDto)
        {
            var userToLogin =await _authService.LoginWithEmailAsync(userMailLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin);
            }
            else
            {
                var result = await _authService.CreateAccessTokenAsync(userToLogin.Data);
                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }


        }
       

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserForRegisterDto userForRegisterDto)
        {



            var registerResult =await  _authService.RegisterAsync(userForRegisterDto);
            if (registerResult.Success)
            {
                var result = await _authService.CreateAccessTokenAsync(registerResult.Data);
                if (result.Success)
                {
                    return Ok(result);
                }
            }



            return BadRequest(Messages.CouldNotCreateUser);
        }
    }
}