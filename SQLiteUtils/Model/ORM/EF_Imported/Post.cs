namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Post")]
    public partial class Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Post()
        {
            Comment = new HashSet<Comment>();
            Image = new HashSet<Image>();
            UserLiked = new HashSet<UserLiked>();
            Hashtag = new HashSet<Hashtag>();
        }

        public long Id { get; set; }

        [StringLength(1000)]
        public string Caption { get; set; }

        public bool? IsPublic { get; set; }

        public long CreatedOn { get; set; }

        public long? LastUpdate { get; set; }

        public long UserId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        public virtual DietPlan DietPlan { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Image> Image { get; set; }

        public virtual MeasuresEntry MeasuresEntry { get; set; }

        public virtual User User { get; set; }

        public virtual TrainingSchedule TrainingSchedule { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLiked> UserLiked { get; set; }

        public virtual UserPhase UserPhase { get; set; }

        public virtual Weight Weight { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hashtag> Hashtag { get; set; }
    }
}