namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ActivityDay")]
    public partial class ActivityDay
    {
        public long Id { get; set; }

        public long? Steps { get; set; }

        public long? CaloriesOut { get; set; }

        public long? Stairs { get; set; }

        public long? SleepMinutes { get; set; }

        public long? SleepQuality { get; set; }

        public long? HeartRateRest { get; set; }

        public long? HeartRateMax { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }
    }
}
