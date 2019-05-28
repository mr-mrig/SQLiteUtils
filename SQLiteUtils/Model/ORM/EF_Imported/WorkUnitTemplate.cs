namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkUnitTemplate")]
    public partial class WorkUnitTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkUnitTemplate()
        {
            LinkedWorkUnitTemplate = new HashSet<LinkedWorkUnitTemplate>();
            LinkedWorkUnitTemplate1 = new HashSet<LinkedWorkUnitTemplate>();
            SetTemplate = new HashSet<SetTemplate>();
        }

        public long Id { get; set; }

        public long ProgressiveNumber { get; set; }

        public long WorkoutTemplateId { get; set; }

        public long ExcerciseId { get; set; }

        public long? WorkUnitTemplateNoteId { get; set; }

        public virtual Excercise Excercise { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SetTemplate> SetTemplate { get; set; }

        public virtual WorkoutTemplate WorkoutTemplate { get; set; }

        public virtual WorkUnitTemplateNote WorkUnitTemplateNote { get; set; }
    }
}
