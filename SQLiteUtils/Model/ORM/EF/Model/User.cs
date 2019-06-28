namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            BiaEntry = new HashSet<BiaEntry>();
            Circumference = new HashSet<Circumference>();
            Comment = new HashSet<Comment>();
            DietHashtag = new HashSet<DietHashtag>();
            DietPlan = new HashSet<DietPlan>();
            ExcercisePersonalLibrary = new HashSet<ExcercisePersonalLibrary>();
            Hashtag = new HashSet<Hashtag>();
            MeasuresEntry = new HashSet<MeasuresEntry>();
            PersonalRecord = new HashSet<PersonalRecord>();
            Phase = new HashSet<Phase>();
            Plicometry = new HashSet<Plicometry>();
            Post = new HashSet<Post>();
            TraineeHasHashtag = new HashSet<TraineeHasHashtag>();
            TraineeHashtag = new HashSet<TraineeHashtag>();
            TrainingCollaboration = new HashSet<TrainingCollaboration>();
            TrainingHashtag = new HashSet<TrainingHashtag>();
            TrainingPlan = new HashSet<TrainingPlan>();
            TrainingScheduleFeedback = new HashSet<TrainingScheduleFeedback>();
            UserHasProficiency = new HashSet<UserHasProficiency>();
            UserHasProficiency1 = new HashSet<UserHasProficiency>();
            UserHasProficiency2 = new HashSet<UserHasProficiency>();
            UserLike = new HashSet<UserLike>();
            UserPhase = new HashSet<UserPhase>();
            UserRelation = new HashSet<UserRelation>();
            UserRelation1 = new HashSet<UserRelation>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Email { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Username { get; set; }

        [Required]
        [StringLength(128)]
        public string Password { get; set; }

        [Required]
        [StringLength(128)]
        public string Salt { get; set; }

        public long SubscriptionDate { get; set; }

        public long? AccountStatusTypeId { get; set; }

        public virtual AccountStatusType AccountStatusType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BiaEntry> BiaEntry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Circumference> Circumference { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietHashtag> DietHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietPlan> DietPlan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExcercisePersonalLibrary> ExcercisePersonalLibrary { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hashtag> Hashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeasuresEntry> MeasuresEntry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonalRecord> PersonalRecord { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phase> Phase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Plicometry> Plicometry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Post { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TraineeHasHashtag> TraineeHasHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TraineeHashtag> TraineeHashtag { get; set; }

        public virtual Trainer Trainer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingCollaboration> TrainingCollaboration { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingHashtag> TrainingHashtag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPlan> TrainingPlan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserHasProficiency> UserHasProficiency { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserHasProficiency> UserHasProficiency1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserHasProficiency> UserHasProficiency2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLike> UserLike { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserPhase> UserPhase { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRelation> UserRelation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRelation> UserRelation1 { get; set; }
    }
}
