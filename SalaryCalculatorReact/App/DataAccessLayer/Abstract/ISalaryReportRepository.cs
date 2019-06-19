using SalaryCalculatorReact.Model;
using SalaryCalculatorReact.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.App.DataAccessLayer
{
    public interface ISalaryReportRepository: IDataAccessLayer<SalaryReport>
    {
        Task<float> CalculateAsyncById(int month, int year,int id);
        Task CalculateAsyncAll(int month, int year);
        Task<List<SalaryReportDto>> GetDtoAllAsync(int month, int year, string AccountId, string role);
    }
}
