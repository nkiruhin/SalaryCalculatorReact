using Newtonsoft.Json;
using SalaryCalculatorReact.App.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.ViewModel
{

    public class SalaryReportDto
    {
        public int Id { set; get; }

        public string DateCreate { set; get; }

        public string EmployeeName { set; get; }

        public string PositionName { set; get; }

        public string Period { set; get; }

        public string SumOfSalary { set; get; }

        public string salaryOfEmploees { set; get; }

    }
}
