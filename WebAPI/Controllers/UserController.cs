using Application.Contracts;
using Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;

        public UserController(IUser user)
        {
            _user = user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _user.LoginUserAsync(loginDTO);

            if (!result.Flag)
            {
                if (result.Message == "Usuário não encontrado!")
                    return NotFound(result); 
                else
                    return Unauthorized(result);
            }

            return Ok(result); 
        }


        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            var result = await _user.RegisterUserAsync(registerUserDTO);
            if (result.Message == "Email já cadastrado")
                return Conflict(result);
            return Ok(result);
        }
    }
}
