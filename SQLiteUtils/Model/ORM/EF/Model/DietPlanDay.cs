namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietPlanDay")]
    public partial class DietPlanDay
    {
        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public long? CarbGrams { get; set; }

        public long? FatGrams { get; set; }

        public long? ProteinGrams { get; set; }

        public long? SaltGrams { get; set; }

        public long? WaterLiters { get; set; }

        public long? SpecificWeekDay { get; set; }

        public long DietPlanUnitId { get; set; }

        public long DietDayTypeId { get; set; }

        public virtual DietDayType DietDayType { get; set; }

        public virtual DietPlanUnit DietPlanUnit { get; set; }
    }
}
