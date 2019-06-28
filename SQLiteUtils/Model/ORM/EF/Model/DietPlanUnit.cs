namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietPlanUnit")]
    public partial class DietPlanUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DietPlanUnit()
        {
            DietPlanDay = new HashSet<DietPlanDay>();
        }

        public long Id { get; set; }

        public long StartDate { get; set; }

        public long? EndDatePlanned { get; set; }

        public long? EndDate { get; set; }

        public long DietPlanId { get; set; }

        public virtual DietPlan DietPlan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietPlanDay> DietPlanDay { get; set; }
    }
}
