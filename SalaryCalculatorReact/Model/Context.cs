using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.Model
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options)
              : base(options)
        { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<SalaryReport> SalaryReport { get; set; }
    }
}
