namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserRelation")]
    public partial class UserRelation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SourceUserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TargetUserId { get; set; }

        public long LastUpdate { get; set; }

        public long RelationStatusId { get; set; }

        public virtual RelationStatus RelationStatus { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
