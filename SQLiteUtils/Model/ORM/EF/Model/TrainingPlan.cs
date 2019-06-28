namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingPlan")]
    public partial class TrainingPlan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TrainingPlan()
        {
            TrainingPlanHasHashtag = new HashSet<TrainingPlanHasHashtag>();
            TrainingPlanRelation = new HashSet<TrainingPlanRelation>();
            TrainingPlanRelation1 = new HashSet<TrainingPlanRelation>();
            TrainingSchedule = new HashSet<TrainingSchedule>();
            TrainingWeek = new HashSet<TrainingWeek>();
            Muscle = new HashSet<Muscle>();
            Phase = new HashSet<Phase>();
            TrainingProficiency = new HashSet<TrainingProficiency>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Name { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        public long IsBookmarked { get; set; }

        public long IsTemplate { get; set; }

        public long CreatedOn { get; set; }

        public long OwnerId { get; set; }

        public long? TrainingPlanNoteId { get; set; }

        public virtual TrainingPlanNote TrainingPlanNote { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPlanHasHashtag> TrainingPlanHasHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPlanRelation> TrainingPlanRelation1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingSchedule> TrainingSchedule { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingWeek> TrainingWeek { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Muscle> Muscle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phase> Phase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingProficiency> TrainingProficiency { get; set; }
    }
}
