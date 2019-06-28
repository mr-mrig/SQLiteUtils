namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("IntensityTechnique")]
    public partial class IntensityTechnique
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IntensityTechnique()
        {
            LinkedWorkUnitTemplate = new HashSet<LinkedWorkUnitTemplate>();
            WorkingSetTemplateIntensityTechnique = new HashSet<WorkingSetTemplateIntensityTechnique>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Name { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Abbreviation { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        public long IsLinkingTechnique { get; set; }

        public long? RPE { get; set; }

        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetTemplateIntensityTechnique> WorkingSetTemplateIntensityTechnique { get; set; }
    }
}
