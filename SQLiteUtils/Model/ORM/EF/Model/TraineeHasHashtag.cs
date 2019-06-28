namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TraineeHasHashtag")]
    public partial class TraineeHasHashtag
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long HashtagId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TraineeId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TrainerId { get; set; }

        public long ProgressiveNumber { get; set; }

        public virtual Trainer Trainer { get; set; }

        public virtual User User { get; set; }

        public virtual TraineeHashtag TraineeHashtag { get; set; }
    }
}
