namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkUnit")]
    public partial class WorkUnit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkUnit()
        {
            WorkingSet = new HashSet<WorkingSet>();
        }

        public long Id { get; set; }

        public long? QuickRating { get; set; }

        public long ProgressiveNumber { get; set; }

        public long WorkoutSessionId { get; set; }

        public long ExcerciseId { get; set; }

        public virtual Excercise Excercise { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSet> WorkingSet { get; set; }

        public virtual WorkoutSession WorkoutSession { get; set; }
    }
}
