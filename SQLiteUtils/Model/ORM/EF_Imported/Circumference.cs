namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Circumference")]
    public partial class Circumference
    {
        public long Id { get; set; }

        public long? Neck { get; set; }

        public long? Chest { get; set; }

        public long? Shoulders { get; set; }

        public long? LeftForearm { get; set; }

        public long? RightForearm { get; set; }

        public long? LeftArm { get; set; }

        public long? RightArm { get; set; }

        public long? Waist { get; set; }

        public long? Hips { get; set; }

        public long? LeftLeg { get; set; }

        public long? RightLeg { get; set; }

        public long? LeftCalf { get; set; }

        public long? RightCalf { get; set; }

        public long? OwnerId { get; set; }

        public virtual User User { get; set; }

        public virtual MeasuresEntry MeasuresEntry { get; set; }
    }
}
