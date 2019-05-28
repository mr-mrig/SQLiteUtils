namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Excercise")]
    public partial class Excercise
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Excercise()
        {
            ExcerciseSecondaryTarget = new HashSet<ExcerciseSecondaryTarget>();
            PersonalRecord = new HashSet<PersonalRecord>();
            WorkUnit = new HashSet<WorkUnit>();
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
            PerformanceFocus = new HashSet<PerformanceFocus>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Name { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        [StringLength(2147483647)]
        public string ExecutionGuide { get; set; }

        [StringLength(2147483647)]
        public string CriticalPointsDescription { get; set; }

        [StringLength(2147483647)]
        public string ImageUrl { get; set; }

        public long? IsApproved { get; set; }

        public long CreatedOn { get; set; }

        public long? LastUpdate { get; set; }

        public long MuscleId { get; set; }

        public long TrainingEquipmentId { get; set; }

        public long OwnerId { get; set; }

        public long ExcerciseDifficultyId { get; set; }

        public long? PerformanceTypeId { get; set; }

        public virtual PerformanceType PerformanceType { get; set; }

        public virtual ExerciseDifficulty ExerciseDifficulty { get; set; }

        public virtual User User { get; set; }

        public virtual TrainingEquipment TrainingEquipment { get; set; }

        public virtual Muscle Muscle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonalRecord> PersonalRecord { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnit> WorkUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PerformanceFocus> PerformanceFocus { get; set; }
    }
}
