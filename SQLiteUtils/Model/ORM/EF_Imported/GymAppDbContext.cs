namespace SQLiteUtils.Model.ORM.EF_Imported
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GymAppDbContext : DbContext
    {
        public GymAppDbContext()
            : base("name=GymAppDbContext")
        {
        }

        public virtual DbSet<AccountStatusType> AccountStatusType { get; set; }
        public virtual DbSet<ActivityDay> ActivityDay { get; set; }
        public virtual DbSet<BiaDevice> BiaDevice { get; set; }
        public virtual DbSet<BiaDeviceBrand> BiaDeviceBrand { get; set; }
        public virtual DbSet<BiaDeviceType> BiaDeviceType { get; set; }
        public virtual DbSet<BiaEntry> BiaEntry { get; set; }
        public virtual DbSet<Circumference> Circumference { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<DietDay> DietDay { get; set; }
        public virtual DbSet<DietDayType> DietDayType { get; set; }
        public virtual DbSet<DietHashtag> DietHashtag { get; set; }
        public virtual DbSet<DietPlan> DietPlan { get; set; }
        public virtual DbSet<DietPlanDay> DietPlanDay { get; set; }
        public virtual DbSet<DietPlanUnit> DietPlanUnit { get; set; }
        public virtual DbSet<EffortType> EffortType { get; set; }
        public virtual DbSet<Excercise> Excercise { get; set; }
        public virtual DbSet<ExcerciseSecondaryTarget> ExcerciseSecondaryTarget { get; set; }
        public virtual DbSet<ExerciseDifficulty> ExerciseDifficulty { get; set; }
        public virtual DbSet<FitnessDayEntry> FitnessDayEntry { get; set; }
        public virtual DbSet<Hashtag> Hashtag { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<IntensityTechnique> IntensityTechnique { get; set; }
        public virtual DbSet<LinkedWorkUnit> LinkedWorkUnit { get; set; }
        public virtual DbSet<LinkedWorkUnitTemplate> LinkedWorkUnitTemplate { get; set; }
        public virtual DbSet<MeasuresEntry> MeasuresEntry { get; set; }
        public virtual DbSet<Mus> Mus { get; set; }
        public virtual DbSet<Muscle> Muscle { get; set; }
        public virtual DbSet<MuscleWorkType> MuscleWorkType { get; set; }
        public virtual DbSet<PerformanceFocus> PerformanceFocus { get; set; }
        public virtual DbSet<PerformanceType> PerformanceType { get; set; }
        public virtual DbSet<PersonalRecord> PersonalRecord { get; set; }
        public virtual DbSet<Phase> Phase { get; set; }
        public virtual DbSet<Plicometry> Plicometry { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<RelationStatus> RelationStatus { get; set; }
        public virtual DbSet<SetTemplate> SetTemplate { get; set; }
        public virtual DbSet<SetTemplateIntensityTechnique> SetTemplateIntensityTechnique { get; set; }
        public virtual DbSet<TrainingEquipment> TrainingEquipment { get; set; }
        public virtual DbSet<TrainingHashtag> TrainingHashtag { get; set; }
        public virtual DbSet<TrainingPlan> TrainingPlan { get; set; }
        public virtual DbSet<TrainingPlanMessage> TrainingPlanMessage { get; set; }
        public virtual DbSet<TrainingPlanNote> TrainingPlanNote { get; set; }
        public virtual DbSet<TrainingPlanRelation> TrainingPlanRelation { get; set; }
        public virtual DbSet<TrainingPlanRelationType> TrainingPlanRelationType { get; set; }
        public virtual DbSet<TrainingProficiency> TrainingProficiency { get; set; }
        public virtual DbSet<TrainingSchedule> TrainingSchedule { get; set; }
        public virtual DbSet<TrainingScheduleFeedback> TrainingScheduleFeedback { get; set; }
        public virtual DbSet<TrainingScheduleHashtag> TrainingScheduleHashtag { get; set; }
        public virtual DbSet<TrainingWeek> TrainingWeek { get; set; }
        public virtual DbSet<TrainingWeekTemplate> TrainingWeekTemplate { get; set; }
        public virtual DbSet<TrainingWeekType> TrainingWeekType { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserLiked> UserLiked { get; set; }
        public virtual DbSet<UserPhase> UserPhase { get; set; }
        public virtual DbSet<UserPhaseNote> UserPhaseNote { get; set; }
        public virtual DbSet<UserRelation> UserRelation { get; set; }
        public virtual DbSet<Weight> Weight { get; set; }
        public virtual DbSet<WellnessDay> WellnessDay { get; set; }
        public virtual DbSet<WorkingSet> WorkingSet { get; set; }
        public virtual DbSet<WorkingSetIntensityTechnique> WorkingSetIntensityTechnique { get; set; }
        public virtual DbSet<WorkingSetNote> WorkingSetNote { get; set; }
        public virtual DbSet<WorkoutSession> WorkoutSession { get; set; }
        public virtual DbSet<WorkoutTemplate> WorkoutTemplate { get; set; }
        public virtual DbSet<WorkUnit> WorkUnit { get; set; }
        public virtual DbSet<WorkUnitTemplate> WorkUnitTemplate { get; set; }
        public virtual DbSet<WorkUnitTemplateNote> WorkUnitTemplateNote { get; set; }
        public virtual DbSet<C_Table1> C_Table1 { get; set; }
        public virtual DbSet<UserHasProficiency> UserHasProficiency { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BiaDeviceBrand>()
                .HasMany(e => e.BiaDevice)
                .WithRequired(e => e.BiaDeviceBrand)
                .HasForeignKey(e => e.BrandId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BiaDeviceType>()
                .HasMany(e => e.BiaDevice)
                .WithRequired(e => e.BiaDeviceType)
                .HasForeignKey(e => e.DeviceTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DietDayType>()
                .HasMany(e => e.DietPlanDay)
                .WithRequired(e => e.DietDayType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DietHashtag>()
                .HasMany(e => e.DietPlan)
                .WithMany(e => e.DietHashtag)
                .Map(m => m.ToTable("DietHasHashtag").MapLeftKey("DietHashtagId").MapRightKey("DietPlanId"));

            modelBuilder.Entity<DietPlan>()
                .HasMany(e => e.DietPlanUnit)
                .WithRequired(e => e.DietPlan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DietPlanUnit>()
                .HasMany(e => e.DietPlanDay)
                .WithRequired(e => e.DietPlanUnit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Excercise>()
                .HasMany(e => e.ExcerciseSecondaryTarget)
                .WithRequired(e => e.Excercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Excercise>()
                .HasMany(e => e.PersonalRecord)
                .WithRequired(e => e.Excercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Excercise>()
                .HasMany(e => e.WorkUnit)
                .WithRequired(e => e.Excercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Excercise>()
                .HasMany(e => e.WorkUnitTemplate)
                .WithRequired(e => e.Excercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Excercise>()
                .HasMany(e => e.PerformanceFocus)
                .WithMany(e => e.Excercise)
                .Map(m => m.ToTable("ExerciseFocus").MapLeftKey("ExerciseId").MapRightKey("PerformanceFocusId"));

            modelBuilder.Entity<ExerciseDifficulty>()
                .HasMany(e => e.Excercise)
                .WithRequired(e => e.ExerciseDifficulty)
                .HasForeignKey(e => e.ExcerciseDifficultyId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FitnessDayEntry>()
                .HasOptional(e => e.ActivityDay)
                .WithRequired(e => e.FitnessDayEntry);

            modelBuilder.Entity<FitnessDayEntry>()
                .HasOptional(e => e.DietDay)
                .WithRequired(e => e.FitnessDayEntry);

            modelBuilder.Entity<FitnessDayEntry>()
                .HasOptional(e => e.Weight)
                .WithRequired(e => e.FitnessDayEntry);

            modelBuilder.Entity<FitnessDayEntry>()
                .HasOptional(e => e.WellnessDay)
                .WithRequired(e => e.FitnessDayEntry);

            modelBuilder.Entity<Hashtag>()
                .HasMany(e => e.Post)
                .WithMany(e => e.Hashtag)
                .Map(m => m.ToTable("PostHasHashtag").MapLeftKey("HashtagId").MapRightKey("PostId"));

            modelBuilder.Entity<IntensityTechnique>()
                .HasMany(e => e.LinkedWorkUnit)
                .WithRequired(e => e.IntensityTechnique)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IntensityTechnique>()
                .HasMany(e => e.LinkedWorkUnitTemplate)
                .WithRequired(e => e.IntensityTechnique)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IntensityTechnique>()
                .HasMany(e => e.SetTemplateIntensityTechnique)
                .WithRequired(e => e.IntensityTechnique)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IntensityTechnique>()
                .HasMany(e => e.WorkingSetIntensityTechnique)
                .WithRequired(e => e.IntensityTechnique)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MeasuresEntry>()
                .HasOptional(e => e.BiaEntry)
                .WithRequired(e => e.MeasuresEntry);

            modelBuilder.Entity<MeasuresEntry>()
                .HasOptional(e => e.Circumference)
                .WithRequired(e => e.MeasuresEntry);

            modelBuilder.Entity<MeasuresEntry>()
                .HasOptional(e => e.Plicometry)
                .WithRequired(e => e.MeasuresEntry);

            modelBuilder.Entity<Mus>()
                .HasMany(e => e.WellnessDay)
                .WithMany(e => e.Mus)
                .Map(m => m.ToTable("WellnessDayHasMus").MapLeftKey("MusId").MapRightKey("WellnessDayId"));

            modelBuilder.Entity<Muscle>()
                .HasMany(e => e.Excercise)
                .WithRequired(e => e.Muscle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Muscle>()
                .HasMany(e => e.ExcerciseSecondaryTarget)
                .WithRequired(e => e.Muscle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Muscle>()
                .HasMany(e => e.TrainingPlan)
                .WithMany(e => e.Muscle)
                .Map(m => m.ToTable("TrainingMuscleFocus").MapLeftKey("MuscleId").MapRightKey("TrainingPlanId"));

            modelBuilder.Entity<Phase>()
                .HasMany(e => e.UserPhase)
                .WithRequired(e => e.Phase)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Phase>()
                .HasMany(e => e.TrainingPlan)
                .WithMany(e => e.Phase)
                .Map(m => m.ToTable("TrainingPlanHasPhase").MapLeftKey("PhaseId").MapRightKey("PlanId"));

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Post)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.DietPlan)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.FitnessDayEntry)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.FitnessDayEntry1)
                .WithRequired(e => e.Post1);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.MeasuresEntry)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.TrainingSchedule)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.UserLiked)
                .WithRequired(e => e.Post)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.UserPhase)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<Post>()
                .HasOptional(e => e.Weight)
                .WithRequired(e => e.Post);

            modelBuilder.Entity<RelationStatus>()
                .HasMany(e => e.UserRelation)
                .WithRequired(e => e.RelationStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SetTemplate>()
                .HasMany(e => e.SetTemplateIntensityTechnique)
                .WithOptional(e => e.SetTemplate)
                .HasForeignKey(e => e.LinkedSetTemplateId);

            modelBuilder.Entity<SetTemplate>()
                .HasMany(e => e.SetTemplateIntensityTechnique1)
                .WithRequired(e => e.SetTemplate1)
                .HasForeignKey(e => e.SetTemplateId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SetTemplate>()
                .HasMany(e => e.WorkingSetIntensityTechnique)
                .WithRequired(e => e.SetTemplate)
                .HasForeignKey(e => e.WorkingSetId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingEquipment>()
                .HasMany(e => e.Excercise)
                .WithRequired(e => e.TrainingEquipment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingHashtag>()
                .HasMany(e => e.TrainingScheduleHashtag)
                .WithRequired(e => e.TrainingHashtag)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingHashtag>()
                .HasMany(e => e.TrainingPlan)
                .WithMany(e => e.TrainingHashtag)
                .Map(m => m.ToTable("TrainingPlanHasHashtag").MapLeftKey("TrainingHashtagId").MapRightKey("TrainingPlanId"));

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(e => e.TrainingPlanRelation)
                .WithRequired(e => e.TrainingPlan)
                .HasForeignKey(e => e.ChildPlanId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(e => e.TrainingPlanRelation1)
                .WithRequired(e => e.TrainingPlan1)
                .HasForeignKey(e => e.ParentPlanId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(e => e.TrainingSchedule)
                .WithRequired(e => e.TrainingPlan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(e => e.TrainingWeekTemplate)
                .WithRequired(e => e.TrainingPlan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingPlanRelationType>()
                .HasMany(e => e.TrainingPlanRelation)
                .WithRequired(e => e.TrainingPlanRelationType)
                .HasForeignKey(e => e.RelationTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingProficiency>()
                .HasMany(e => e.TrainingPlan)
                .WithMany(e => e.TrainingProficiency)
                .Map(m => m.ToTable("TrainingPlanTarget").MapLeftKey("TrainingProficiencyId").MapRightKey("TrainingPlanId"));

            modelBuilder.Entity<TrainingSchedule>()
                .HasMany(e => e.TrainingScheduleFeedback)
                .WithRequired(e => e.TrainingSchedule)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingSchedule>()
                .HasMany(e => e.TrainingScheduleHashtag)
                .WithRequired(e => e.TrainingSchedule)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingSchedule>()
                .HasMany(e => e.TrainingWeek)
                .WithRequired(e => e.TrainingSchedule)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrainingWeekTemplate>()
                .HasMany(e => e.WorkoutTemplate)
                .WithRequired(e => e.TrainingWeekTemplate)
                .HasForeignKey(e => e.TrainingWeekId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.BiaEntry)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Circumference)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DietHashtag)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.DietPlan)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Excercise)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.IntensityTechnique)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.PersonalRecord)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Phase)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Plicometry)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Post)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingHashtag)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingPlan)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingProficiency)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrainingScheduleFeedback)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserHasProficiency)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.OwnerId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserHasProficiency1)
                .WithOptional(e => e.User1)
                .HasForeignKey(e => e.ProficiencyId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserHasProficiency2)
                .WithOptional(e => e.User2)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserLiked)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserPhase)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRelation)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.TargetUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRelation1)
                .WithRequired(e => e.User1)
                .HasForeignKey(e => e.SourceUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkingSet>()
                .HasMany(e => e.WorkingSetIntensityTechnique)
                .WithOptional(e => e.WorkingSet)
                .HasForeignKey(e => e.LinkedWorkingSetId);

            modelBuilder.Entity<WorkingSet>()
                .HasMany(e => e.WorkingSetNote)
                .WithRequired(e => e.WorkingSet)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkoutSession>()
                .HasMany(e => e.TrainingWeek)
                .WithOptional(e => e.WorkoutSession)
                .HasForeignKey(e => e.NextWorkoutId);

            modelBuilder.Entity<WorkoutSession>()
                .HasMany(e => e.WorkUnit)
                .WithRequired(e => e.WorkoutSession)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkoutTemplate>()
                .HasMany(e => e.WorkUnitTemplate)
                .WithRequired(e => e.WorkoutTemplate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnit>()
                .HasMany(e => e.LinkedWorkUnit)
                .WithRequired(e => e.WorkUnit)
                .HasForeignKey(e => e.SecondWorkUnitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnit>()
                .HasMany(e => e.LinkedWorkUnit1)
                .WithRequired(e => e.WorkUnit1)
                .HasForeignKey(e => e.FirstWorkUnitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnit>()
                .HasMany(e => e.WorkingSet)
                .WithRequired(e => e.WorkUnit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnitTemplate>()
                .HasMany(e => e.LinkedWorkUnitTemplate)
                .WithRequired(e => e.WorkUnitTemplate)
                .HasForeignKey(e => e.SecondWorkUnitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnitTemplate>()
                .HasMany(e => e.LinkedWorkUnitTemplate1)
                .WithRequired(e => e.WorkUnitTemplate1)
                .HasForeignKey(e => e.FirstWorkUnitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkUnitTemplate>()
                .HasMany(e => e.SetTemplate)
                .WithRequired(e => e.WorkUnitTemplate)
                .HasForeignKey(e => e.WorkUnitId)
                .WillCascadeOnDelete(false);
        }
    }
}