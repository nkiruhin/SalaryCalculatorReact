using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using SalaryCalculatorReact.App.Auth.Abstract;
using SalaryCalculatorReact.App.DataAccessLayer;
using SalaryCalculatorReact.Controllers;
using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel.Mapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    
    public class PositionControllerTest
    {
        private readonly IModelMetadataProvider _provider;
        private Context _context;
        private PositionController _positionController;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employees;
        private readonly IAuthService _authService;
        public PositionControllerTest()
        {
            _provider = new EmptyModelMetadataProvider();
            _context = new Context(InMemoryContext.NewContext());
            _mapper = new Mapper(new MapperConfiguration(mc => mc.AddProfile(new EmployeeProfile())));
            _employees = new EmployeeRepository(_context, _provider, _mapper);
            InMemoryContext.SeedData(_context);
            _authService = new Mock<IAuthService>().Object;
            _positionController = new PositionController(new PositionRepository(_context, _provider,_mapper,_employees), new Mock<IAuthService>().Object);           
        }


        [Theory(DisplayName = "Тест получения формы из контролера")]
        [InlineData(1)]
        [InlineData(null)]
        public void GetForm(int? id)
        {
            // Arrange
           
           
            // Act
            var form = _positionController.GetForm(id).Value.ToList();
            // Assert
            Assert.Equal(9, form.Count);
            if (id == null) {           
                Assert.Null(form.Find(f => f.Name == "PositionName").Value);
            }else
            {
                Assert.Equal("Employee", form.Find(f => f.Name == "PositionName").Value);
            }
            
        }
        
        [Fact(DisplayName = "Тест получения списка должностей из контролера")]
        
        public void GetPositions()
        {
            // Arrange
           
            // Act
            var positions = _positionController.GetPositions().Value.ToList();
            // Assert
            Assert.Equal(3,positions.Count());
        }
        [Theory(DisplayName = "Тест получения должности по Id из контролера")]
        [InlineData(0)]
        [InlineData(1)]
        public void GetPosition(int id)
        {
            // Arrange
            // Act
            ActionResult<Position> position = _positionController.GetPosition(id).Result;
            // Assert
            if (position.Value != null) { 
                Assert.Equal(1, position.Value.Id);
            }
            else
            {
                Assert.Equal("Microsoft.AspNetCore.Mvc.NotFoundResult", position.Result.ToString());
            }
        }
        [Fact(DisplayName = "Тест добавления должности")]
        public async void PostPositionAsync()
        {
            // Arrange
            Position position = new Position
            {
                 Id = 4,
                 PositionName ="Recruter",
                 SalaryRate =16000,
                 Koeff=4
            };
            // Act
            await _positionController.PostPosition(position);
            // Assert
            var positionId = _context.Position.Single(i => i.Id == 4)?.Id;
            Assert.Equal(4, positionId);
        }
        [Fact(DisplayName = "Тест редактирования должности ")]
        public async void PutEmployeeAsync()
        {
            // Arrange
            Position position = new Position
            {
                Id = 1,
                PositionName="Recruter",
                Koeff =13000
            };
            // Act
            await _positionController.PutPosition(1, position);

            // Assert
            var newposition = _context.Position.Single(i => i.Id == 1);
            Assert.Equal(13000, newposition.Koeff);
            Assert.Equal("Recruter", newposition.PositionName);
        }
        [Theory(DisplayName = "Тест удаления должности ")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async void DeleteEmployeeAsync(int id)
        {
            // Arrange

            // Act
            //await _positionController.DeletePosition(id);
            
            ActionResult<Position> result = null;

            //Exception test
            
            //Exception ex = null;
            //if (id == 1)
            //{
            //    ex = await Assert.ThrowsAsync<Exception>(async () => await _positionController.DeletePosition(id));
            //}

              result = await _positionController.DeletePosition(id);
            // Assert
            if (id == 0)
            {
                Assert.Equal("Microsoft.AspNetCore.Mvc.NotFoundResult", result.Result.ToString());
            }
            if (id == 1)
            {
                Assert.Equal("Microsoft.AspNetCore.Mvc.BadRequestObjectResult", result.Result.ToString());
            }

        }
    }
}
