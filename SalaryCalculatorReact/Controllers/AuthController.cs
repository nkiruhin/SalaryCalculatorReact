using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculatorReact.App;
using SalaryCalculatorReact.App.Auth.Abstract;


namespace SalaryCalculatorReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {

        
        private readonly IAuthService _authService;


        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet("Token")]
        [AllowAnonymous]
        public async Task<IActionResult> Token(string username, string password)
        {
            var result = await _authService.LoginAsync(username, password);
            if (!result.Succeeded)
            {
                return StatusCode(401, new { error = "Неверный логин/пароль" });
            }           
            // создаем JWT-токен
            var access_token = await _authService.GetAuthDataAsync(username);
            return Ok(access_token);
        }

        [HttpGet("Logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Успешно" });
        }
        [HttpGet("Form/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<Field>>> GetFormAsync(int id)
        {

            return await _authService.GetFormAsync(id);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> EditEployeeAccount(int id, [FromForm] string username, [FromForm] string password)
        {
            var result = await _authService.EditAccount(id, username, password);
            if (!result.Succeeded)
            {
               return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            return Ok(new { message = "Учетная запись добавлена" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteEployeeAccount(int id)
        {
            var result = await _authService.DeleteAccount(id);
            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }
            return Ok(new { message = "Учетная запись удалена" });
        }
        
    }
}