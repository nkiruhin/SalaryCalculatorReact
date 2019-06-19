using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Controllers;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel.Mapper;
using System;
using System.Linq;
using Xunit;

namespace XUnitTest
{
    public class SalaryReportControllerTest
    {
        private readonly IModelMetadataProvider _provider;
        private readonly Context _context;
        private SalaryReportController _salaryreportController;
        private ISalaryReportRepository _salaryReportRepository;
        private readonly MapperConfiguration _mappercfg;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employees;
        public SalaryReportControllerTest()
        {
            
            _provider = new EmptyModelMetadataProvider();
            _context = new Context(InMemoryContext.NewContext());
            _mappercfg = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new SalaryReportProfile());
                mc.AddProfile(new EmployeeProfile());
            });
            _mapper = new Mapper(_mappercfg);
            _employees = new EmployeeRepository(_context, _provider, _mapper);
            InMemoryContext.SeedData(_context);
            _salaryReportRepository = new SalaryReportRepository(_context, _provider, _mapper, _employees);
            _salaryreportController = new SalaryReportController(_context,_salaryReportRepository);
        }
       
        
        [Fact(DisplayName = "Тест получения отчета из контролера")]
        public async void GetSalaryReportsAsync()
        {
            // Arrange

            // Act
            var salaryreports = await _salaryReportRepository.GetDtoAllAsync(1, 2019, "", "Administrator");
            // Assert
            //Assert.Equal(3,salaryreports.Count());
        }
        [Fact(DisplayName = "Тест расчета")]
        public async void CalculateAsync()
        {
            // Arrange
            await _salaryreportController.Calculate(3, 2019);
            // Act

            // Assert
            //Assert.Equal(3,salaryreports.Count());
        }


    }
}
