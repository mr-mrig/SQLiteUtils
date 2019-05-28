namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BiaDevice")]
    public partial class BiaDevice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BiaDevice()
        {
            BiaEntry = new HashSet<BiaEntry>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(2147483647)]
        public string Model { get; set; }

        [StringLength(2147483647)]
        public string Name { get; set; }

        public long BrandId { get; set; }

        public long DeviceTypeId { get; set; }

        public virtual BiaDeviceType BiaDeviceType { get; set; }

        public virtual BiaDeviceBrand BiaDeviceBrand { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BiaEntry> BiaEntry { get; set; }
    }
}
