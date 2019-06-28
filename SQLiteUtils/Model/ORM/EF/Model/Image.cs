namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Image")]
    public partial class Image
    {
        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Url { get; set; }

        public long IsProgressPicture { get; set; }

        public long PostId { get; set; }

        public virtual Post Post { get; set; }
    }
}
