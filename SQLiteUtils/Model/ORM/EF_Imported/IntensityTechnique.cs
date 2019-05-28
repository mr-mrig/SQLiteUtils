namespace SQLiteUtils.Model.ORM.EF_Imported
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
            LinkedWorkUnit = new HashSet<LinkedWorkUnit>();
            LinkedWorkUnitTemplate = new HashSet<LinkedWorkUnitTemplate>();
            SetTemplateIntensityTechnique = new HashSet<SetTemplateIntensityTechnique>();
            WorkingSetIntensityTechnique = new HashSet<WorkingSetIntensityTechnique>();
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

        public long? IsApproved { get; set; }

        public long CreatedOn { get; set; }

        public long OwnerId { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedWorkUnit> LinkedWorkUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SetTemplateIntensityTechnique> SetTemplateIntensityTechnique { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkingSetIntensityTechnique> WorkingSetIntensityTechnique { get; set; }
    }
}
