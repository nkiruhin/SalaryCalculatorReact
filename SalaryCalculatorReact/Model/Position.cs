using SalaryCalculatorReact.App.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SalaryCalculatorReact.Model
{
    public class Position:IModelBase
    {
        [ScaffoldColumn(false)]
        [Column("PositionId")]
        public int Id { set; get; }
        [DisplayName("Должность")]
        [Required]
        public string PositionName { set; get; }
        [DisplayName("Базовая ставка")]
        public float SalaryRate { set; get; }
        [DisplayName("% за каждый год работы")]
        public int? LongevityKoeff { set; get; }
        [DisplayName("Максимальный % за выслугу лет")]
        public int? MaxLongevityKoeff { set; get; }
        [DisplayName("Надбавка за подчиненных?")]
        public bool IsChildrenSalary { set; get; }       
        [DisplayName("Все уровни подчиненности?")]
        public bool IsAllChildrenSalary { set; get; }
        [DisplayName("Процент с з/п подчиненных")]
        public float? Koeff { set; get; }
        [ScaffoldColumn(false)]
        [NotMapped] 
        public string Name => PositionName;
    }
}
