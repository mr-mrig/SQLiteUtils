namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PostHasHashtag")]
    public partial class PostHasHashtag
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PostId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long HashtagId { get; set; }

        public long ProgressiveNumber { get; set; }

        public virtual Hashtag Hashtag { get; set; }

        public virtual Post Post { get; set; }
    }
}
