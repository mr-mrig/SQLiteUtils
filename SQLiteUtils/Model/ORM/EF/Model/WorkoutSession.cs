namespace SQLiteUtils
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
            WorkUnit = new HashSet<WorkUnit>();
        }

        public long Id { get; set; }

        public long? PlannedDate { get; set; }

        public long StartTime { get; set; }

        public long? EndTime { get; set; }

        public long? Rating { get; set; }

        public long? WorkoutTemplateId { get; set; }

        public virtual Post Post { get; set; }

        public virtual WorkoutTemplate WorkoutTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnit> WorkUnit { get; set; }
    }
}
