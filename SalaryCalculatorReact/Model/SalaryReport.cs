using SalaryCalculatorReact.App.DataAccessLayer;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SalaryCalculatorReact.Model
{
    public class SalaryReport:IModelBase
    {
        [Column("SalaryReportId")]
        public int Id { set; get; }
        [DisplayName("Дата отчета")]
        [DataType(DataType.Date)]
        public DateTime DateCreate { set; get; }
        [DisplayName("Дата обновления")]
        [DataType(DataType.Date)]
        public DateTime DateUpdate { set; get; }
        [DisplayName("Месяц")]
        [Range(1, 12, ErrorMessage = "Месяц должен не должен быть больше 12")]
        public int month { set; get; }
        [DisplayName("Год")]
        [Range(1900, 2019, ErrorMessage = "Не верно введен год")]
        public int year { set; get; }
        public int EmployeeId { set; get; }
        [DisplayName("Сотрудник")]
        public Employee Employee { set; get; }
        [DisplayName("Сумма")]
        public float SumOfSalary { set; get; }
        [DisplayName("З/П сотрудников")]
        public float? salaryOfEmploees { set; get; }
        /// <summary>
        /// property for list generator
        /// </summary>
        [NotMapped]  
        public string Name { get; }
    }
}
