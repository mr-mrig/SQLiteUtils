namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WorkUnitTemplateNote")]
    public partial class WorkUnitTemplateNote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkUnitTemplateNote()
        {
            WorkUnitTemplate = new HashSet<WorkUnitTemplate>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Body { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkUnitTemplate> WorkUnitTemplate { get; set; }
    }
}
