using AutoMapper;
using SalaryCalculatorReact.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SalaryCalculatorReact.App.DataAccessLayer.SalaryReportRepository;

namespace SalaryCalculatorReact.ViewModel.Mapper
{
    public class EmployeeProfile:Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest=>dest.Account,opt=>opt.MapFrom(src=>src.AccountId!=null?true:false));
            CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToShortDateString());
            CreateMap<Employee, sreport>()
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.IsAllChildrenSalary, opt => opt.MapFrom(src => src.Position.IsAllChildrenSalary));
        }
    }
}
