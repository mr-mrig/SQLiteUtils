namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BiaEntry")]
    public partial class BiaEntry
    {
        public long Id { get; set; }

        public long? Kg { get; set; }

        public long? Bf { get; set; }

        public long? Tbw { get; set; }

        public long? Ecw { get; set; }

        public long? EcMatrix { get; set; }

        public long? Bmr { get; set; }

        public long? Hpa { get; set; }

        public long? BiaDeviceId { get; set; }

        public long? OwnerId { get; set; }

        public virtual BiaDevice BiaDevice { get; set; }

        public virtual User User { get; set; }

        public virtual MeasuresEntry MeasuresEntry { get; set; }
    }
}
