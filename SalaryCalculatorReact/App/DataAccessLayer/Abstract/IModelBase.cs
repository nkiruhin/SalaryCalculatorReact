using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    /// <summary>
    /// Базовый интерфейс для объектов БД
    /// </summary>
    public interface IModelBase
    {
            int Id { get; set; }
            string Name { get; }
    }
}
