namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserDetail")]
    public partial class UserDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public long? Birthday { get; set; }

        public long? Height { get; set; }

        [StringLength(2147483647)]
        public string About { get; set; }

        public long? GenderTypeId { get; set; }

        public virtual GenderType GenderType { get; set; }

        public virtual User User { get; set; }
    }
}
