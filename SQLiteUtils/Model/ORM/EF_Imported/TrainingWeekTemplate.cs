namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingWeekTemplate")]
    public partial class TrainingWeekTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingWeekTemplate()
        {
            WorkoutTemplate = new HashSet<WorkoutTemplate>();
        }

        public long Id { get; set; }

        public long ProgressiveNumber { get; set; }

        public long? TrainingWeekTypeId { get; set; }

        public long TrainingPlanId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual TrainingWeekType TrainingWeekType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkoutTemplate> WorkoutTemplate { get; set; }
    }
}
