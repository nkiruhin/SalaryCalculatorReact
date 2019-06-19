using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
   
    public class SalaryReportRepository: DataAccessLayer<SalaryReport>,ISalaryReportRepository
    {
        private readonly IEmployeeRepository _employee;
        public SalaryReportRepository(Context context, IModelMetadataProvider provider, IMapper mapper, IEmployeeRepository employee) : base(context, provider, mapper)
        {
            _employee = employee;
           
        }
        private async Task<List<sreport>> GetAllemployeesAsync(Employee employee, int month, int year)
        {
            List<sreport> allChildrens = new List<sreport>(); // Инициализируем список для промежуточных данных                                                                 
            List<sreport> childrens = await _employee.GetDtoAllAsync<sreport>(n => n.ManagerId == employee.Id); // Получаем список подчиненных первого уровня
            allChildrens.AddRange(childrens); // Добавляем в результурующий список всех подчиненых первого уровня
            foreach (sreport child in childrens) //В цикле рачитываем зарплату для каждого подчиненного
            {
                var sr = GetSingle(
                    n => n.EmployeeId == child.Employee.Id
                    && n.month == month
                    && n.year == year); //  Проверяем наличие расчета в БД за данный период
                if (sr != null)
                {
                    child.SumOfSalary = sr.SumOfSalary;  // Берем из БД
                }
                else
                { // Расчитываем если нет в БД
                    child.SumOfSalary = await CalculateAsyncById(month, year, child.Employee.Id);
                }
                if (child.IsAllChildrenSalary) // Запускаем рекурсию уходя ниже по дереву
                {
                    List<sreport> childList = await GetAllemployeesAsync(child.Employee, month, year);
                    allChildrens.AddRange(childList);// Добавляем в результурующий список
                }

            }
            return allChildrens; // Возращаем список отчетов по подчиненным
        }
        /// <summary>
        /// Класс для хранения промеуточных данных расчета
        /// </summary>
        public class sreport
        {
            public Employee Employee { set; get; }
            public float? SumOfSalary { set; get; }
            public int PositionId { set; get; }
            public bool IsAllChildrenSalary { set; get; }
        }
        public async Task CalculateAsyncAll(int month, int year)
        {
            var employees = _employee.GetAll();
            foreach (Employee employee in employees)
            {
                SalaryReport salaryReport = GetSingle(n => n.EmployeeId == employee.Id && n.month == month && n.year == year);
                if (salaryReport == null)
                {
                   var summ= await CalculateAsyncById(month, year, employee.Id);
                }
            }
        }
            /// <summary>
            /// Расчет з/п для конкретного сотрудника. 
            /// Расчитывает з.п подчиненных если определено должностью и сохраняет в БД
            /// </summary>
            /// <param name="month">месяц</param>
            /// <param name="year">год</param>
            /// <param name="id">id сотрудника</param>
            /// <returns>Возращает сумму з.п</returns>
        public async Task<float> CalculateAsyncById(int month, int year,int id)
        {
            float koeffemployees=0, salaryOfEmploees=0, payforEmployees=0;
            List<sreport> sreport = new List<sreport>();
            Employee employee = _employee.GetSingle(n => n.Id == id, n => n.Position,p=>p.Position);
            var startPeriodDay = new DateTime(year, month, 1);
            var YearSpan = startPeriodDay.Year - employee.DateofRecruitment.Year;
            int stage = startPeriodDay.AddYears(YearSpan) <= employee.DateofRecruitment ? YearSpan : YearSpan - 1;
            if (startPeriodDay < employee.DateofRecruitment) return 0; // Не работал на данный период
            int LongevitProcent = (int)employee.Position.LongevityKoeff * stage;
            LongevitProcent = LongevitProcent >= employee.Position.MaxLongevityKoeff ?(int)employee.Position.MaxLongevityKoeff : LongevitProcent;
            if (employee.Position.IsChildrenSalary)
            {
                sreport = await GetAllemployeesAsync(employee, month, year);
                if (!employee.Position.IsAllChildrenSalary)
                {
                    sreport = sreport.Where(n => n.Employee.ManagerId == id).ToList(); // Убираем данные не нужные для расчета
                }
                koeffemployees = (float)employee.Position.Koeff;
            }
            if (sreport.Count > 0)
            {
                salaryOfEmploees = (float)sreport?.Sum(n => n.SumOfSalary);
                payforEmployees = koeffemployees * salaryOfEmploees / 100;
            }
            float Summ = employee.Position.SalaryRate * (1 + LongevitProcent / 100) + payforEmployees;
            await AddAsync( new SalaryReport
            {
                EmployeeId = id,
                month = month,
                year = year,
                DateCreate = DateTime.Now,
                SumOfSalary = Summ,
                salaryOfEmploees = salaryOfEmploees
            });
            await CommitAsync();
            return Summ;
        }
        /// <summary>
        /// /Проход по подчиненным с вызовом метода расчета для каждого из подчиненных
        /// </summary>
        /// <param name="employee">Объект сотрудника</param>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        /// <returns>Возврашает массив объектов SalaryReport с расчитанами зарплатами </returns>
        public async Task<List<SalaryReportDto>> GetDtoAllAsync(int month,int year,string AccountId,string role)
        {
           
            if (role == "Administrator") {
               
                return  await base.GetDtoAllAsync<SalaryReportDto>(n => n.month == month && n.year == year);

            } else
            {
                var employee = _employee.GetSingle(e => e.AccountId == AccountId, p => p.Position);
                var employees = await _employee.GetDtoAllAsync(employee, role);
                return await base.GetDtoAllAsync<SalaryReportDto>(sr => sr.month == month 
                                                                        && sr.year == year 
                                                                        && employees.Any(e => e.Id == sr.EmployeeId));               
            }
            
        }
    }
}
