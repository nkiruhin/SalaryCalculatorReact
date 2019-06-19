using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class SalaryReportController : ControllerBase
    {
        private readonly ISalaryReportRepository _salaryreport;
        public SalaryReportController(Context context,ISalaryReportRepository salaryreport)
        {
            _salaryreport = salaryreport;
        }

        // GET: api/SalaryReport

        [HttpGet]
        public async Task<ActionResult<List<SalaryReportDto>>> GetSalaryReports([FromQuery]int month, int year)
        {           
            var role = HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            string AccountId = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var report = await _salaryreport.GetDtoAllAsync(month, year, AccountId, role);
            if (report.Count()==0)
            {
                return NotFound(new { error = "Отчет за данный период отсутствует в системе. Нажмите \"Расчет з/п\" на панели" });
            }
            return report;

        }
        
        // POST: api/SalaryReport/Calculate

        [HttpPost("Calculate")]
        public async Task<ActionResult> Calculate([FromQuery]int month, int year)
        {
            await _salaryreport.CalculateAsyncAll(month, year);
            
            return Ok(new { message = "Расчет завершен. Нажмите \"Показать отчет\"" });
        }

    }
}
