using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Controllers;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel;
using SalaryCalculatorReact.ViewModel.Mapper;
using System;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class EmployeeControllerTest
    {
        private readonly IModelMetadataProvider _provider;
        private Context _context;
        private EmployeesController _employeeController;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _repositary;
        public EmployeeControllerTest()
        {
            _provider = new EmptyModelMetadataProvider();
            _context = new Context(InMemoryContext.NewContext());
            _mapper = new Mapper(new MapperConfiguration(mc => mc.AddProfile(new EmployeeProfile())));
            InMemoryContext.SeedData(_context);
            _repositary = new EmployeeRepository(_context, _provider, _mapper);
            _employeeController = new EmployeesController(_repositary);
        }
        [Theory(DisplayName = "Тест получения формы из контролера")]
        [InlineData(1)]
        [InlineData(null)]
        public void GetForm(int? id)
        {
            // Arrange

            // Act
            var form = _employeeController.GetForm(id).Value.ToList();
            // Assert
            Assert.Equal(12, form.Count);
            if (id == null) {           
                Assert.Null(form.Find(f => f.Name == "FirstName").Value);
            }else
            {
                Assert.Equal("Иван", form.Find(f => f.Name == "FirstName").Value);
            }
            
        }
        
        [Fact(DisplayName = "Тест получения списка сотрудников")]
        public async void GetEmployeesAsync()
        {
            // Arrange

            // Act
            //var employees = await _employeeController.GetEmployeesAsync();
            Employee employee = null;
            var employees = await _repositary.GetDtoAllAsync(employee, "Administrator");
            // Assert
            Assert.Equal(4,employees.Count);
        }
        [Theory(DisplayName = "Тест получения сотрудника по Id из контролера")]
        [InlineData(0)]
        [InlineData(1)]
        public void GetEmployee(int id)
        {
            // Arrange
            // Act
            ActionResult<Employee> employee = _employeeController.GetEmployee(id).Result;
            // Assert
            if (employee.Value != null) { 
                Assert.Equal(1,employee.Value.Id);
            }
            else
            {
                Assert.Equal("Microsoft.AspNetCore.Mvc.NotFoundResult", employee.Result.ToString());
            }
        }
        [Fact(DisplayName = "Тест добавления сотрудника ")]
        public async void PostEmployeeAsync()
        {
            // Arrange
            Employee employee = new Employee
            {
                  Id = 5, Surname = "Пупкин", FirstName = "Вадим",
                  LastName = "Павлович", BirthDay = new DateTime(1999, 07, 12),
                  DateofRecruitment = new DateTime(2007, 7, 1), PositionId = 2 
            };
            // Act
            await _employeeController.PostEmployee(employee);
            // Assert
            var employeeId = _context.Employees.Single(i => i.Id == 2)?.Id;
            Assert.Equal(2, employeeId);
        }
        [Fact(DisplayName = "Тест редактирования сотрудника ")]
        public async void PutEmployeeAsync()
        {
            // Arrange
            Employee employee = new Employee
            {
                Id = 1,
                LastName = "Иванович",
                ManagerId = 2
            };
            // Act
            await _employeeController.PutEmployee(1, employee);

            // Assert
            var newemployee = _context.Employees.Single(i => i.Id == 1);
            Assert.Equal(2, newemployee.ManagerId);
            Assert.Equal("Иванович", newemployee.LastName);
        }
    }
}
