namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SetTemplate")]
    public partial class SetTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SetTemplate()
        {
            SetTemplateIntensityTechnique = new HashSet<SetTemplateIntensityTechnique>();
            SetTemplateIntensityTechnique1 = new HashSet<SetTemplateIntensityTechnique>();
            WorkingSetIntensityTechnique = new HashSet<WorkingSetIntensityTechnique>();
        }

        public long Id { get; set; }

        public long ProgressiveNumber { get; set; }

        [StringLength(2147483647)]
        public string TargetRepetitions { get; set; }

        public long? Rest { get; set; }

        [StringLength(2147483647)]
        public string Cadence { get; set; }

        public long? Effort { get; set; }

        public long WorkUnitId { get; set; }

        public long? EffortTypeId { get; set; }

        public virtual EffortType EffortType { get; set; }

        public virtual WorkUnitTemplate WorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SetTemplateIntensityTechnique> SetTemplateIntensityTechnique { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SetTemplateIntensityTechnique> SetTemplateIntensityTechnique1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetIntensityTechnique> WorkingSetIntensityTechnique { get; set; }
    }
}
