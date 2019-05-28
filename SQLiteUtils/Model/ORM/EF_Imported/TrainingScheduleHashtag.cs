namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingScheduleHashtag")]
    public partial class TrainingScheduleHashtag
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TrainingScheduleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TrainingHashtagId { get; set; }

        public long? DisplayOrder { get; set; }

        public virtual TrainingHashtag TrainingHashtag { get; set; }

        public virtual TrainingSchedule TrainingSchedule { get; set; }
    }
}
