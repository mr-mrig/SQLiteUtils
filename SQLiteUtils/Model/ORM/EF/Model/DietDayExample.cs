namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietDayExample")]
    public partial class DietDayExample
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DietDayExample()
        {
            DietDayMealExample = new HashSet<DietDayMealExample>();
            DietPlan = new HashSet<DietPlan>();
        }

        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Introduction { get; set; }

        [StringLength(2147483647)]
        public string PrivateNote { get; set; }

        public long? DietDayTypeId { get; set; }

        public virtual DietDayType DietDayType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietDayMealExample> DietDayMealExample { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietPlan> DietPlan { get; set; }
    }
}
