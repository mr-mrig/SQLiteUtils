namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkingSetIntensityTechnique")]
    public partial class WorkingSetIntensityTechnique
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long WorkingSetId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long IntensityTechniqueId { get; set; }

        public long? LinkedWorkingSetId { get; set; }

        public virtual IntensityTechnique IntensityTechnique { get; set; }

        public virtual SetTemplate SetTemplate { get; set; }

        public virtual WorkingSet WorkingSet { get; set; }
    }
}
