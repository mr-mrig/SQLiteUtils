using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{

    [Table("FitnessDayEntry")]
    public class FitnessDayEntry : Post
    {


        public virtual int DayDate { get; set; }
        public virtual byte Rating { get; set; }

        public virtual Post Post { get; set; }
        public virtual Weight Weight { get; set; }
        public virtual DietDay DietDay { get; set; }
        public virtual ActivityDay ActivityDay { get; set; }
        public virtual Wellness Wellness { get; set; }
    }
}
