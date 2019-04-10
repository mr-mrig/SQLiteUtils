using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace SQLiteUtils.Model.Wrappers
{
    public class DbWrapperTrainingProfile
    {


        #region Consts

        /// <summary>
        /// Number of days which a period si made up by
        /// </summary>
        public const ushort DaysPerPeriod = 7;
        #endregion



        #region Private Fields
        #endregion



        #region Properties

        /// <summary>
        /// Default duration [Weeks] of Training Plan perdiods (default = 5)
        /// </summary>
        public byte DefaultTrainingPlanPeriod { get; set; } = 5;

        /// <summary>
        /// Duration [Weeks] of the usual Training Plan (which might be randomicly changed accordint to the OffestMin/Max parameters)
        /// </summary>
        public byte TrainingPlanPeriod { get; set; }

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte TrainingPlanPeriodOffsetMin { get; set; } = 3;

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte TrainingPlanPeriodOffsetMax { get; set; } = 3;

        /// <summary>
        /// Date which the user took measures last time
        /// </summary>
        public DateTime TrainingPlanStarted { get; private set; }

        /// <summary>
        /// Relation Type Id
        /// </summary>
        public TrainingPlanRelationWrapper.RelationType RelationType { get; set; } = TrainingPlanRelationWrapper.RelationType.None;

        /// <summary>
        /// Number of weeks
        /// </summary>
        public byte WeeksNum { get; private set; }


        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte WorkoutsOffsetMin { get; set; } = 2;

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte WorkoutsOffsetMax { get; set; } = 2;

        /// <summary>
        /// Number of workouts
        /// </summary>
        public byte WorkoutsNum { get; private set; } = 4;
        
        /// <summary>
        /// Probability a workout will increase its work units number each week (to increase the volume over the weeks)
        /// </summary>
        public float WeeklyWorkUnitIncreaseProbability { get; private set; } = 0.6f;

        /// <summary>
        /// Workouts per training week
        /// </summary>
        public Dictionary<byte, List<DbWrapperWorkoutProfile>> Workouts { get; set; }
        #endregion



        #region Ctors

        public DbWrapperTrainingProfile(DateTime startDate)
        {
            TrainingPlanStarted = startDate;
            TrainingPlanPeriod = (byte)RandomFieldGenerator.RandomInt(DefaultTrainingPlanPeriod - TrainingPlanPeriodOffsetMin, TrainingPlanPeriodOffsetMax + 1);

            WeeksNum = (byte)RandomFieldGenerator.ChooseAmong(new List<int?>() { 1, TrainingPlanPeriod }).Value;
            WeeksNum = 4;
            WorkoutsNum = (byte)RandomFieldGenerator.RandomInt(WorkoutsNum - WorkoutsOffsetMin, WorkoutsNum + WorkoutsOffsetMax + 1);

            // TODO: Only Variant relation type supported so far
            RelationType = RandomFieldGenerator.RandomDouble(0, 1) < 0.2f ? TrainingPlanRelationWrapper.RelationType.Variant : TrainingPlanRelationWrapper.RelationType.None;


            // Build the workouts for all the weeks of the plan
            Workouts = BuildWorkouts();
        }
        #endregion



        #region Public Methods


        /// <summary>
        /// Tells wheter it's time to change the Training Plan.
        /// </summary>
        /// <param name="currentDate">Date to be checked</param>
        /// <returns></returns>
        public bool IsExpired(DateTime currentDate)
        {
            return TrainingPlanStarted.AddDays(DaysPerPeriod * TrainingPlanPeriod) < currentDate;
            //bool ret;

            //ret = TrainingPlanStarted.AddDays(DaysPerPeriod * TrainingPlanPeriod) >= currentDate;

            //if (ret)
            //{
            //    TrainingPlanStarted = currentDate;
            //    TrainingPlanPeriod = (byte)RandomFieldGenerator.RandomInt(
            //        DefaultTrainingPlanPeriod - TrainingPlanPeriodOffsetMin, TrainingPlanPeriodOffsetMax + 1);
            //}

            //return ret;
        }


        /// <summary>
        /// Get the type of the selected week
        /// </summary>
        /// <param name="iWeek">Progressive number of the week</param>
        /// <returns>Week type ID</returns>
        public WeekTemplateWrapper.WeekType GetWeekType(byte iWeek)
        {

            if (iWeek == 0 && RandomFieldGenerator.RandomDouble(0, 1) < 0.5f)
                return WeekTemplateWrapper.WeekType.Deload;

            else if (iWeek == WeeksNum - 1 && RandomFieldGenerator.RandomDouble(0, 1) < 0.5f)
                return WeekTemplateWrapper.WeekType.Peak;

            else
                return WeekTemplateWrapper.WeekType.None;
        }

        #endregion



        #region Private Methods

        private Dictionary<byte, List<DbWrapperWorkoutProfile>> BuildWorkouts()
        {
            Dictionary<byte, List<DbWrapperWorkoutProfile>> ret = new Dictionary<byte, List<DbWrapperWorkoutProfile>>();
            DateTime lastWorkout = TrainingPlanStarted;

            // Week 0
            List<DbWrapperWorkoutProfile>  workouts =  new List<DbWrapperWorkoutProfile>();

            for (byte i = 0; i < WorkoutsNum; i++)
            {
                workouts.Add(new DbWrapperWorkoutProfile()
                {
                    OrderNumber = i,
                    StartTime = lastWorkout,
                    EndTime = lastWorkout.AddHours(RandomFieldGenerator.RandomDouble(0.8, 2)),
                });
                lastWorkout = lastWorkout.AddDays(1);
            }
            ret.Add(0, workouts);

            // Next weeks
            for (byte iWeek = 1; iWeek < WeeksNum; iWeek++)
            {
                workouts = new List<DbWrapperWorkoutProfile>();

                // Add as many days as needed to go to the following week (considering that Workouts are on subsequent days)
                lastWorkout = lastWorkout.AddDays(8 - (WorkoutsNum % 8) - 1);

                for (byte i = 0; i < WorkoutsNum; i++)
                {
                    // Increase the volume (in terms of WU) each week with a certain probability
                    byte workUnitsNum = (byte)(ret[(byte)(iWeek - 1)].Select(x => x.WorkUnitsNum).ToList()[i]
                        + Convert.ToByte(RandomFieldGenerator.RandomBoolWithProbability(WeeklyWorkUnitIncreaseProbability)));

                    workouts.Add(new DbWrapperWorkoutProfile(workUnitsNum)
                    {
                        OrderNumber = i,
                        StartTime = lastWorkout,
                        EndTime = lastWorkout.AddHours(RandomFieldGenerator.RandomDouble(0.8, 2)),
                    });
                    lastWorkout.AddDays(i);
                }

                ret.Add(iWeek, workouts);
            }

            return ret;
        }
        #endregion

    }
}
