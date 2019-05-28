namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkoutSession")]
    public partial class WorkoutSession
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkoutSession()
        {
            TrainingWeek = new HashSet<TrainingWeek>();
            WorkUnit = new HashSet<WorkUnit>();
        }

        public long Id { get; set; }

        public long? PlannedDate { get; set; }

        public long StartTime { get; set; }

        public long? EndTime { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public long? Rating { get; set; }

        public long? TrainingWeekId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingWeek> TrainingWeek { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnit> WorkUnit { get; set; }
    }
}
