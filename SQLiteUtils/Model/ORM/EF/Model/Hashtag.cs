namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Hashtag")]
    public partial class Hashtag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hashtag()
        {
            PostHasHashtag = new HashSet<PostHasHashtag>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Body { get; set; }

        public long EntryStatusTypeId { get; set; }

        public long? ModeratorId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PostHasHashtag> PostHasHashtag { get; set; }
    }
}
