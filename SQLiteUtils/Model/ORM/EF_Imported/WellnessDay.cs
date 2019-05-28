namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WellnessDay")]
    public partial class WellnessDay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WellnessDay()
        {
            Mus = new HashSet<Mus>();
        }

        public long Id { get; set; }

        [Column(TypeName = "real")]
        public double? Temperature { get; set; }

        public long? Glycemia { get; set; }

        public virtual FitnessDayEntry FitnessDayEntry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mus> Mus { get; set; }
    }
}
