namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietPlan")]
    public partial class DietPlan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DietPlan()
        {
            DietPlanUnit = new HashSet<DietPlanUnit>();
            DietHashtag = new HashSet<DietHashtag>();
            DietDayExample = new HashSet<DietDayExample>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Name { get; set; }

        public long? WeeklyFreeMealsNumber { get; set; }

        public long CreatedOn { get; set; }

        [StringLength(2147483647)]
        public string OwnerNote { get; set; }

        public long? OwnerId { get; set; }

        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietPlanUnit> DietPlanUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietHashtag> DietHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietDayExample> DietDayExample { get; set; }
    }
}
