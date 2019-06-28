namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietDayMealExample")]
    public partial class DietDayMealExample
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DietDayExampleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MealExampleId { get; set; }

        public long? MealTypeId { get; set; }

        public virtual DietDayExample DietDayExample { get; set; }

        public virtual MealType MealType { get; set; }

        public virtual MealExample MealExample { get; set; }
    }
}
