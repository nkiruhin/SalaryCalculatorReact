using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    public class PositionRepository : DataAccessLayer<Position>,IPositionRepository
    {
        private readonly IEmployeeRepository _employee;
        public PositionRepository(Context context, IModelMetadataProvider provider,IMapper mapper, IEmployeeRepository employee) : base(context,provider,mapper) {
            _employee = employee;
        }
        public override void Delete(Position entity)
        {
            Expression<Func<Employee, bool>> expression = n => n.PositionId == entity.Id;
            if (_employee.FindBy(expression).Count() > 0)
            {
                throw new Exception("Удаление не возможно. Имеются сотрудники с данной должностью");
            }

            base.Delete(entity);

        }
    }
    
}
