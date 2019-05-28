namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Weight")]
    public partial class Weight
    {
        public long Id { get; set; }

        public long Kg { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

        public virtual Post Post { get; set; }
    }
}
