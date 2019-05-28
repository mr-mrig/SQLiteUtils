using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{
    public class DietDayType
    {

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<DietDay> DietDays { get; set; }
    }
}
