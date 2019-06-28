namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Body { get; set; }

        public long CreatedOn { get; set; }

        public long UserId { get; set; }

        public long PostId { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }
    }
}
