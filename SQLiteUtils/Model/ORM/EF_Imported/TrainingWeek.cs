namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingWeek")]
    public partial class TrainingWeek
    {
        public long Id { get; set; }

        public long ProgressiveNumber { get; set; }

        public long TrainingScheduleId { get; set; }

        public long? TrainingWeekTypeId { get; set; }

        public long? NextWorkoutId { get; set; }

        public virtual TrainingSchedule TrainingSchedule { get; set; }

        public virtual WorkoutSession WorkoutSession { get; set; }

        public virtual TrainingWeekType TrainingWeekType { get; set; }
    }
}
