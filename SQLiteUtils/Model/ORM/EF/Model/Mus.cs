namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Mus
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mus()
        {
            WellnessDay = new HashSet<WellnessDay>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Name { get; set; }

        [StringLength(2147483647)]
        public string Description { get; set; }

        public long EntryStatusTypeId { get; set; }

        public virtual EntryStatusType EntryStatusType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WellnessDay> WellnessDay { get; set; }
    }
}
