using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    public class EmployeeRepository : DataAccessLayer<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(Context context, IModelMetadataProvider provider,IMapper mapper) : base(context,provider,mapper) { }
        
        public IEnumerable<Employee> ManagersList(string term) // For serverSide filter
        {
            var list = this.AllIncluding(p => p.Position).Where(n => n.Position.PositionName != "Employee"&&n.Name.Contains(term));
            return list;
        }
        /// <summary>
        /// Cписок подчиненных сотрудника в зависимости от его должности
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<List<EmployeeDto>> GetDtoAllAsync(Employee employee,string role)
        {
            if (role == "Administrator") // Администратору возращем весь список сотрудников
            {
                return await base.GetDtoAllAsync<ViewModel.EmployeeDto>();
            }
            else
            {
                if (employee.Position.IsAllChildrenSalary) // Возращаем потомков всех уровней, если это указано в должности (роли)
                {
                    var employees = new List<ViewModel.EmployeeDto>
                    {
                        GetDto<ViewModel.EmployeeDto>(employee.Id)
                    };
                    getChildrenAsync(employee.Id, employees);
                    return employees;
                }
                else if (employee.Position.IsChildrenSalary) // Возращаем потомков первого уровня, если это указано в должности (роли)
                {
                    return await base.GetDtoAllAsync<ViewModel.EmployeeDto>(n => n.ManagerId == employee.Id || n.Id == employee.Id);
                    
                }
                else // Возращаем только данные о самом сотруднике.
                {
                    return new List<ViewModel.EmployeeDto> { base.GetDto<ViewModel.EmployeeDto>(employee.Id) };
                }
            }
        }
        private void getChildrenAsync(int id, List<EmployeeDto> employees)
        {
            var childrens = base.GetDtoAllAsync<EmployeeDto>(e => e.ManagerId == id).Result;
            employees.AddRange(childrens);
            foreach (var children in childrens)
            {
                getChildrenAsync(children.Id, employees);
            }
        }
    }

}
