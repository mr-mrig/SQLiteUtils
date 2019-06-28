namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExcerciseRelation")]
    public partial class ExcerciseRelation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ParentExcerciseId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChildExcerciseId { get; set; }

        [StringLength(2147483647)]
        public string AdditionalNotes { get; set; }

        public virtual Excercise Excercise { get; set; }

        public virtual Excercise Excercise1 { get; set; }
    }
}
