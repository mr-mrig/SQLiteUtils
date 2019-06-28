namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingSet")]
    public partial class WorkingSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkingSet()
        {
            WorkingSetNote = new HashSet<WorkingSetNote>();
        }

        public long Id { get; set; }

        public long WorkUnitId { get; set; }

        public long ProgressiveNumber { get; set; }

        public long? Repetitions { get; set; }

        public long? Kg { get; set; }

        public virtual WorkUnit WorkUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetNote> WorkingSetNote { get; set; }
    }
}
