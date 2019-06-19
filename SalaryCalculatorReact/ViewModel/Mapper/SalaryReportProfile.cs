using AutoMapper;
using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.ViewModel.Mapper
{
    public class SalaryReportProfile:Profile
    {
        private string RoundToString(float? summ)
        {
            if (summ == 0)
            {
                return "0.00";
            }
            return Math.Round((decimal)summ, 2).ToString("0.00", CultureInfo.InvariantCulture.NumberFormat);
        }
        public SalaryReportProfile()
        {
            CreateMap<SalaryReport, SalaryReportDto>()
            .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.month.ToString() + "." + src.year.ToString()))
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Employee.Position.Name));
            CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToShortDateString());
            CreateMap<float?, string>().ConvertUsing(f => RoundToString(f));
            CreateMap<float, string>().ConvertUsing(f => RoundToString(f));
            CreateMap<EmployeeDto, SalaryReportDto>();
        }
    }
}
