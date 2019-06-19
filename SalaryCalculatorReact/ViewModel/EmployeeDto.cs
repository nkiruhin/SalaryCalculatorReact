using Newtonsoft.Json;
using SalaryCalculatorReact.App.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.ViewModel
{

    public class EmployeeDto:IModelBase
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public string BirthDay { set; get; }

        public string DateofRecruitment { set; get; }

        public string ManagerName { set; get; }
 
        public string PositionName { set; get; }

        public bool Account  { set; get; }
    }
}
