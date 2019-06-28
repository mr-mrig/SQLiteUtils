namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FitnessDayEntry")]
    public partial class FitnessDayEntry
    {
        public long Id { get; set; }

        public long DayDate { get; set; }

        public long? Rating { get; set; }

        public virtual ActivityDay ActivityDay { get; set; }

        public virtual DietDay DietDay { get; set; }

        public virtual Post Post { get; set; }

        public virtual Post Post1 { get; set; }

        public virtual Weight Weight { get; set; }

        public virtual WellnessDay WellnessDay { get; set; }
    }
}
