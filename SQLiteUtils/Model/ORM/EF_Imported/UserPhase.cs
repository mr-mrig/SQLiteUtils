namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserPhase")]
    public partial class UserPhase
    {
        public long Id { get; set; }

        public long StartDate { get; set; }

        public long? EndDate { get; set; }

        public long CreatedOn { get; set; }

        public long OwnerId { get; set; }

        public long PhaseId { get; set; }

        public long? UserPhaseNoteId { get; set; }

        public virtual Phase Phase { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }

        public virtual UserPhaseNote UserPhaseNote { get; set; }
    }
}
