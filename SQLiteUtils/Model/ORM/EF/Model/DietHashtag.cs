namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DietHashtag")]
    public partial class DietHashtag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DietHashtag()
        {
            DietPlan = new HashSet<DietPlan>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Body { get; set; }

        public long EntryStatusTypeId { get; set; }

        public long ModeratorId { get; set; }

        public virtual User User { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DietPlan> DietPlan { get; set; }
    }
}
