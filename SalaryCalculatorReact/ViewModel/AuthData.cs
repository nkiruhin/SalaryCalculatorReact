using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.ViewModel
{
    public class AuthData
    {      
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
        public string name { get; set; }
    }
}
