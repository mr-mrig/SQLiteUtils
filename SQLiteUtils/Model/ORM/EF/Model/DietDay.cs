namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietDay")]
    public partial class DietDay
    {
        public long Id { get; set; }

        public long? CarbGrams { get; set; }

        public long? FatGrams { get; set; }

        public long? ProteinGrams { get; set; }

        public long? SaltGrams { get; set; }

        public long? WaterLiters { get; set; }

        public long? IsFreeMeal { get; set; }

        public long? DietDayTypeId { get; set; }

        public virtual DietDayType DietDayType { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }
    }
}
