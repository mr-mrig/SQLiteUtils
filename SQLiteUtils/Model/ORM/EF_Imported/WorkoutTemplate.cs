namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkoutTemplate")]
    public partial class WorkoutTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkoutTemplate()
        {
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
        }

        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public long IsWeekDaySpecific { get; set; }

        public long ProgressiveNumber { get; set; }

        public long TrainingWeekId { get; set; }

        public virtual TrainingWeekTemplate TrainingWeekTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }
    }
}
