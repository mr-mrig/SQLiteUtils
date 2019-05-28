namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LinkedWorkUnit")]
    public partial class LinkedWorkUnit
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

        public virtual WorkUnit WorkUnit { get; set; }

        public virtual WorkUnit WorkUnit1 { get; set; }
    }
}
