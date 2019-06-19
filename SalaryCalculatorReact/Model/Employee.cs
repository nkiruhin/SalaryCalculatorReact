using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SalaryCalculatorReact.App.DataAccessLayer;
using Newtonsoft.Json;

namespace SalaryCalculatorReact.Model
{
        public class Employee:IModelBase
        {

            [ScaffoldColumn(false)]
            [Column("EmployeeId")]
            public int Id { get; set; }
            [DisplayName("Фамилия")]
            [Required(ErrorMessage ="Фамилия обязательна")]
            public string Surname { get; set; }
            [Required(ErrorMessage = "Имя обязательно")]
            [DisplayName("Имя")]
            public string FirstName { get; set; }
            [DisplayName("Отчество")]
            public string LastName { get; set; }
            [DisplayName("Дата рождения")]
            [DataType(DataType.Date)]
            public DateTime BirthDay { set; get; }
            [DisplayName("Дата приема на работу")]
            [DataType(DataType.Date)]
            public DateTime DateofRecruitment { set; get; }
            [ScaffoldColumn(false)]
            public int? ManagerId { set; get; }
            [DisplayName("Начальник")]
            public Employee Manager { set; get; }
            [ScaffoldColumn(false)]
            [Required(ErrorMessage = "Должность обязательна")]
            public int PositionId { set; get; }
            [DisplayName("Должность")]            
            public Position Position { set; get; }
            [ScaffoldColumn(false)]
            
            public string AccountId { set; get; }
            [NotMapped]
            [ScaffoldColumn(false)]
            public string Name => $"{FirstName} {LastName} {Surname}";

        //[DisplayName("Зарплата")]
        //public List<SalaryReport> Salarys { set; get; }
    }
    }

