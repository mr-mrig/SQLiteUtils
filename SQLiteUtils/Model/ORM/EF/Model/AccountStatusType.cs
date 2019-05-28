using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{
    public class AccountStatusType
    {


        public virtual int Id { get; set; }
        public virtual string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
