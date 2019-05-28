using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{

    [Table("DietDay")]
    public class DietDay : FitnessDayEntry
    {


        public virtual int? CarbGrams {get; set;}
        public virtual int? FatGrams { get; set; }
        public virtual int? ProteinGrams { get; set; }
        public virtual int? SaltGrams { get; set; }
        public virtual int? WaterLiters { get; set; }
        public virtual bool? IsFreeMeal { get; set; }

        public virtual DietDayType DietDayType { get; set; }
        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

    }
}
