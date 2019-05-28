using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model.ORM.EF
{


    [Table("Post")]
    public abstract class Post
    {


        public virtual int Id { get; set; }
        public virtual string Caption { get; set; }
        public virtual int IsPublic { get; set; }
        public virtual int CreatedOn { get; set; }
        public virtual int? LastUpdate { get; set; }

        public virtual User User { get; set; }
        public virtual FitnessDayEntry fitnessDayEntry { get; set; }

    }

}
