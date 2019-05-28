using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{

    [Table("Wellness")]
    public class Wellness : FitnessDayEntry
    {


        public virtual int Temperature { get; set; }
        public virtual int Glycemia { get; set; }

        public virtual ICollection<Mus> Muses { get; set; } 
        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

    }
}
