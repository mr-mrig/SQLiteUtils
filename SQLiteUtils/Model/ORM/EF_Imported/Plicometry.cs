namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Plicometry")]
    public partial class Plicometry
    {
        public long Id { get; set; }

        public long? Chest { get; set; }

        public long? Tricep { get; set; }

        public long? Armpit { get; set; }

        public long? Subscapular { get; set; }

        public long? Suprailiac { get; set; }

        public long? Abdomen { get; set; }

        public long? Thigh { get; set; }

        public long? Kg { get; set; }

        public long? Bf { get; set; }

        public long? OwnerId { get; set; }

        public virtual MeasuresEntry MeasuresEntry { get; set; }

        public virtual User User { get; set; }
    }
}
