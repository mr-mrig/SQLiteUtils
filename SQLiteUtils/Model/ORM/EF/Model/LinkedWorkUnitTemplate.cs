namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LinkedWorkUnitTemplate")]
    public partial class LinkedWorkUnitTemplate
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long FirstWorkUnitId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SecondWorkUnitId { get; set; }

        public long IntensityTechniqueId { get; set; }

        public virtual IntensityTechnique IntensityTechnique { get; set; }

        public virtual WorkUnitTemplate WorkUnitTemplate { get; set; }

        public virtual WorkUnitTemplate WorkUnitTemplate1 { get; set; }
    }
}
