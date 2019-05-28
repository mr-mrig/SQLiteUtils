using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model.ORM.EF
{


    [Table("User")]
    public class User
    {



        #region Accunt Status Types

        //public enum AccontStatusType 


        #endregion


        public virtual int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Salt { get; set; }
        //public AccountStatusType AccountStatusType { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
