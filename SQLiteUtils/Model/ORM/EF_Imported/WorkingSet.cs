namespace SQLiteUtils.Model.ORM.EF_Imported
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
            WorkingSetIntensityTechnique = new HashSet<WorkingSetIntensityTechnique>();
            WorkingSetNote = new HashSet<WorkingSetNote>();
        }

        public long Id { get; set; }

        public long ProgressiveNumber { get; set; }

        [StringLength(2147483647)]
        public string RepetitionsTarget { get; set; }

        public long? Repetitions { get; set; }

        public long? Rest { get; set; }

        [StringLength(2147483647)]
        public string Cadence { get; set; }

        public long? Effort { get; set; }

        public long? KgTarget { get; set; }

        public long? Kg { get; set; }

        public long WorkUnitId { get; set; }

        public long? EffortTypeId { get; set; }

        public virtual EffortType EffortType { get; set; }

        public virtual WorkUnit WorkUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetIntensityTechnique> WorkingSetIntensityTechnique { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetNote> WorkingSetNote { get; set; }
    }
}
