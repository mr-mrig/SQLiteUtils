namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingSetTemplateIntensityTechnique")]
    public partial class WorkingSetTemplateIntensityTechnique
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SetTemplateId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IntensityTechniqueId { get; set; }

        public long? LinkedSetTemplateId { get; set; }

        public virtual IntensityTechnique IntensityTechnique { get; set; }

        public virtual WorkingSetTemplate WorkingSetTemplate { get; set; }

        public virtual WorkingSetTemplate WorkingSetTemplate1 { get; set; }
    }
}
