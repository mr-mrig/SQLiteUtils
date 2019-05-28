namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonalRecord")]
    public partial class PersonalRecord
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long ExcerciseId { get; set; }

        public long RecordDate { get; set; }

        public long Value { get; set; }

        public virtual Excercise Excercise { get; set; }

        public virtual User User { get; set; }
    }
}
