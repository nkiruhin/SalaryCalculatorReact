using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    public interface IPositionRepository:IDataAccessLayer<Position>
    {
       new void Delete(Position entity);
    }
     
}
