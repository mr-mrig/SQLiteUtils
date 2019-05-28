namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingSchedule")]
    public partial class TrainingSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingSchedule()
        {
            TrainingScheduleFeedback = new HashSet<TrainingScheduleFeedback>();
            TrainingScheduleHashtag = new HashSet<TrainingScheduleHashtag>();
            TrainingWeek = new HashSet<TrainingWeek>();
        }

        public long Id { get; set; }

        public long StartDate { get; set; }

        public long? EndDate { get; set; }

        public long? PlannedEndDate { get; set; }

        public long TrainingPlanId { get; set; }

        public long? CurrentWeekId { get; set; }

        public long? PhaseId { get; set; }

        public long? TrainingProficiencyId { get; set; }

        public virtual Phase Phase { get; set; }

        public virtual Post Post { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual TrainingProficiency TrainingProficiency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingScheduleHashtag> TrainingScheduleHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingWeek> TrainingWeek { get; set; }
    }
}
