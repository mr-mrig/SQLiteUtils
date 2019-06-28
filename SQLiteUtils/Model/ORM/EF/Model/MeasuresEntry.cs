namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MeasuresEntry")]
    public partial class MeasuresEntry
    {
        public long Id { get; set; }

        public long MeasureDate { get; set; }

        [StringLength(2147483647)]
        public string OwnerNote { get; set; }

        public long? Rating { get; set; }

        public long OwnerId { get; set; }

        public virtual BiaEntry BiaEntry { get; set; }

        public virtual Circumference Circumference { get; set; }

        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

        public virtual Plicometry Plicometry { get; set; }
    }
}
