namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("_Table1")]
    public partial class C_Table1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Userid { get; set; }
    }
}
