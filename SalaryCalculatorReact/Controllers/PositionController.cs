using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalaryCalculatorReact.App;
using SalaryCalculatorReact.App.Auth.Abstract;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Model;

namespace SalaryCalculatorReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles ="Administrator")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionRepository _position;
        private readonly IAuthService _authService;
        public PositionController(IPositionRepository position,IAuthService authService)
        {
            _position = position;
            _authService = authService;
        }
        // GET: api/Position
        [HttpGet]
        public ActionResult<List<Position>> GetPositions() => _position.GetAll();

        // GET: api/Position/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Position>> GetPosition(int id)
        {
            var position = await _position.GetSingleAsync(id);
            if (position != null)
            {
                return position;
            }
            return NotFound();
        }
        // GET: api/Position/Form/5
        [HttpGet("Form/{id}")]
        public ActionResult<IEnumerable<Field>> GetForm(int? id) => _position.Form(id).ToList(); 
        

        

        // DELETE: api/Position/5
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<Position>> DeletePosition(int id)
        {
            var position = _position.GetSingleNoTracking(id);
            if (position == null)
            {
                return NotFound();
            }
            try
            {
              _position.Delete(position);           
              await _position.CommitAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { errors = "Произошла ошибка. "+ex.Message });
            }
            await _authService.DeleteRole(position.Name);
            return Content(JsonConvert.SerializeObject(new { message = "Удаление завершено" }));

        }
        // POST: api/Position
        [HttpPost]
        public async Task<ActionResult<Position>> PostPosition([FromForm]Position position)
        {
            await _position.AddAsync(position);
            try
            {
                await _position.CommitAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { errors = new[] { "При добавлении произошла ошибка" } });

            }
            await _authService.AddRole(position.Name);
            return StatusCode(201, new { message = "Должность добавлена" });
        }
        // PUT: api/Position/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPosition(int id,[FromForm] Position position)
        {
            var positionForUpdate = _position.GetSingleNoTracking(id);
            if (positionForUpdate == null)
            {
                return NotFound(new { errors = "Неверный идентификатор должности" });
            }
            position.Id = id;
            _position.Update(position);
            try
            {
                await _position.CommitAsync();
            }
            catch (Exception)
            {
                return BadRequest(new {  errors = new [] { "При сохранении произошла ошибка"} });

            }
            return Ok(new { message = "Сохранение выполнено" });
        }
    }
}
