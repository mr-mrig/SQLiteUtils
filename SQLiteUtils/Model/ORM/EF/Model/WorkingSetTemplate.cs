namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingSetTemplate")]
    public partial class WorkingSetTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkingSetTemplate()
        {
            WorkingSetTemplateIntensityTechnique = new HashSet<WorkingSetTemplateIntensityTechnique>();
            WorkingSetTemplateIntensityTechnique1 = new HashSet<WorkingSetTemplateIntensityTechnique>();
        }

        public long Id { get; set; }

        public long WorkUnitTemplateId { get; set; }

        public long ProgressiveNumber { get; set; }

        [StringLength(2147483647)]
        public string TargetRepetitions { get; set; }

        public long? Rest { get; set; }

        [StringLength(2147483647)]
        public string Cadence { get; set; }

        public long? Effort { get; set; }

        public long? EffortTypeId { get; set; }

        public virtual EffortType EffortType { get; set; }

        public virtual WorkUnitTemplate WorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechnique { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechnique1 { get; set; }
    }
}
