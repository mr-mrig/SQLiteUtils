namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingCollaboration")]
    public partial class TrainingCollaboration
    {
        public long Id { get; set; }

        public long StartDate { get; set; }

        public long? EndDate { get; set; }

        public long? ExpirationDate { get; set; }

        [StringLength(2147483647)]
        public string TrainerNote { get; set; }

        [StringLength(2147483647)]
        public string TraineeNote { get; set; }

        public long TrainerId { get; set; }

        public long TraineeId { get; set; }

        public virtual Trainer Trainer { get; set; }

        public virtual User User { get; set; }
    }
}
