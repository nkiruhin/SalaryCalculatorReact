using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalaryCalculatorReact.App;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel;

namespace SalaryCalculatorReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employee;
        public EmployeesController(IEmployeeRepository employee)
        {

            _employee = employee;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployeesAsync() {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            string AccountId = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var employee = _employee.GetSingle(e => e.AccountId == AccountId,p=>p.Position);
            return await _employee.GetDtoAllAsync(employee, role);
        } 

        // GET: api/Employees/Managers
        [HttpGet("Managers")]
        public ActionResult<IEnumerable<Employee>> GetManagers(string term) => _employee.ManagersList(term).ToList();

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            
            var employee = await _employee.GetSingleAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }
        [HttpGet("Form/{id}")]
        public ActionResult<IEnumerable<Field>> GetForm(int? id)
        {
            Expression<Func<Employee, bool>> managersFilter = e => e.Position.PositionName != "Employee"&&e.Id!=id;           
            return _employee.Form(id,managersFilter).ToList();
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutEmployee(int id, [FromForm]Employee employee)
        {
            var employeeForUpdate = _employee.GetSingleNoTracking(id);
            if (employeeForUpdate == null)
            {
                    return NotFound(new { errors = "Неверный идентификатор пользователя" });
            }
            employee.Id = id;   
            _employee.Update(employee);
            try
            {
                await _employee.CommitAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { errors = "При сохранении произошла ошибка" });

            }
            return Ok(new { message ="Сохранение выполнено" });
        }

        // POST: api/Employees
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Employee>> PostEmployee([FromForm]Employee employee)
        {
            await _employee.AddAsync(employee);
            try
            {
                await _employee.CommitAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { errors = "При добавлении произошла ошибка" });

            }
            return StatusCode(201, new { message = "Сотрудник добавлен" });
            
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [Produces("application/json")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var employee = _employee.GetSingleNoTracking(id);
            if (employee == null)
            {
                return NotFound();
            }

            _employee.Delete(employee);
            try {
              await _employee.CommitAsync();
            }
            catch(Exception)
            {
                return BadRequest(new { errors = "Удаление невозможно. Имеются подчиненные" });

            }

            return Content(JsonConvert.SerializeObject(new { message = "Удаление завершено", employee }));

        }

        
    }
}
