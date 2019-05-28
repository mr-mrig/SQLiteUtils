using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{

    [Table("Weight")]
    public class Weight : FitnessDayEntry
    {


        public virtual int Kg { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

    }
}
