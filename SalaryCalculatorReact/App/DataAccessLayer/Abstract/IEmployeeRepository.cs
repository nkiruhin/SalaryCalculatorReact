using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    public interface IEmployeeRepository:IDataAccessLayer<Employee>
    {
        IEnumerable<Employee> ManagersList(string term);
        Task<List<ViewModel.EmployeeDto>> GetDtoAllAsync(Employee employee, string role);
    }
}
