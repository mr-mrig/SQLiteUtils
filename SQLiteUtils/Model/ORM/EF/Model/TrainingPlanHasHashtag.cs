namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingPlanHasHashtag")]
    public partial class TrainingPlanHasHashtag
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TrainingPlanId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TrainingHashtagId { get; set; }

        public long ProgressiveNumber { get; set; }

        public virtual TrainingHashtag TrainingHashtag { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }
    }
}
