namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExcercisePersonalLibrary")]
    public partial class ExcercisePersonalLibrary
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ExcerciseId { get; set; }

        public long IsStarred { get; set; }

        public virtual Excercise Excercise { get; set; }

        public virtual User User { get; set; }
    }
}
