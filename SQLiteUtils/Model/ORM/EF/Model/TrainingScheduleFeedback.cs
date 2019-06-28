namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingScheduleFeedback")]
    public partial class TrainingScheduleFeedback
    {
        public long Id { get; set; }

        [StringLength(2147483647)]
        public string Comment { get; set; }

        public long LastUpdate { get; set; }

        public long? Rating { get; set; }

        public long TrainingScheduleId { get; set; }

        public long UserId { get; set; }

        public virtual TrainingSchedule TrainingSchedule { get; set; }

        public virtual User User { get; set; }
    }
}
