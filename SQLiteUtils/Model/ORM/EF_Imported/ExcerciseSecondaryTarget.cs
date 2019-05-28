namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExcerciseSecondaryTarget")]
    public partial class ExcerciseSecondaryTarget
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ExcerciseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MuscleId { get; set; }

        public long? MuscleWorkTypeId { get; set; }

        public virtual Excercise Excercise { get; set; }

        public virtual MuscleWorkType MuscleWorkType { get; set; }

        public virtual Muscle Muscle { get; set; }
    }
}
