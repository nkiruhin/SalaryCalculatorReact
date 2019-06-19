using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTest
{
    class InMemoryContext 
    {
        public static DbContextOptions<Context> NewContext()
        {
            return new DbContextOptionsBuilder<Context>()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }
        public static void SeedData(Context context)
        {
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            List<Position> Positions = new List<Position>() {
               new Position(){ Id=1,PositionName="Employee", SalaryRate=10000,LongevityKoeff=3, MaxLongevityKoeff=30},
               new Position(){ Id=2,PositionName="Manager", SalaryRate=20000,LongevityKoeff=5,MaxLongevityKoeff=40, IsChildrenSalary=true, Koeff=0.5f},
               new Position(){ Id=3,PositionName="Salesman", SalaryRate=15000,LongevityKoeff=1,MaxLongevityKoeff=40, IsChildrenSalary=true, Koeff=0.3f, IsAllChildrenSalary=true},

            };

            context.Position.AddRange(Positions);
            context.Employees.AddRange(new List<Employee>() {
              new Employee() { Id = 1, Surname = "Сидоров", FirstName = "Иван",
                  LastName = "Петрович", BirthDay = new DateTime(1983, 05, 14),
                  DateofRecruitment = new DateTime(2003, 7, 3), PositionId = 2 },
              new Employee() { Id = 2, Surname = "Ярочкин", FirstName = "Пахом",
                  LastName = "Вячеславович", BirthDay = new DateTime(1973, 03, 10),
                  DateofRecruitment = new DateTime(2013, 5, 2), PositionId = 1,ManagerId=1 },
               new Employee() { Id = 3, Surname = "Самошин", FirstName = "Изяслав",
                  LastName = "Панкратиевич", BirthDay = new DateTime(1985, 07, 19),
                  DateofRecruitment = new DateTime(2011, 11, 3), PositionId = 1,ManagerId=4 },
               new Employee() { Id = 4, Surname = "Чистякова", FirstName = "Валентина",
                  LastName = "Вячеславовна", BirthDay = new DateTime(1983, 05, 26),
                  DateofRecruitment = new DateTime(2008, 1, 3), PositionId = 3,ManagerId=1 },
        });
        context.SalaryReport.Add(new SalaryReport
            {
                Id=1,
                DateCreate= new DateTime(2019, 3, 3),
                DateUpdate = new DateTime(2019, 3, 3),
                EmployeeId = 1,
                month = 1,
                year = 2019,
                SumOfSalary = 12960.01F,
                salaryOfEmploees = 112000.89F                
            });
            context.SaveChanges();
        }
    }
}
