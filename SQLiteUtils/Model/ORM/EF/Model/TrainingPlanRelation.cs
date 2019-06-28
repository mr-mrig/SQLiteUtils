namespace SQLiteUtils
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TrainingPlanRelation")]
    public partial class TrainingPlanRelation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ParentPlanId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChildPlanId { get; set; }

        public long RelationTypeId { get; set; }

        public long? TrainingPlanMessageId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual TrainingPlan TrainingPlan1 { get; set; }

        public virtual TrainingPlanMessage TrainingPlanMessage { get; set; }

        public virtual TrainingPlanRelationType TrainingPlanRelationType { get; set; }
    }
}
