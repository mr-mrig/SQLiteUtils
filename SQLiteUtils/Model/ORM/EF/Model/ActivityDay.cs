using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{

    [Table("ActivityDay")]
    public class ActivityDay : FitnessDayEntry
    {


        public virtual int? Steps { get; set; }
        public virtual int? CaloriesOut { get; set; }
        public virtual int? SleepHours { get; set; }
        public virtual int? HeartRateMax { get; set; }
        public virtual int? HeartRateRest { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

    }
}
