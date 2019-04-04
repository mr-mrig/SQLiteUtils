using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model.Wrappers
{



    public class DbWrapperPostConfig
    {

        public float PostsPerDay { get; set; } = 0.05f;
        public float WeightProbability { get; set; } = 0.4f;
        public float WellnessProbability { get; set; } = 0f;
        public float ActivityDayProbability { get; set; } = 0.25f;
        public float DietDayProbability { get; set; } = 0.4f;



        /// <summary>
        /// Base probability of writing a Post tuned according to a very low user activity degree.
        /// </summary>
        public DbWrapperPostConfig()
        {

        }


    }


    public class DbWrapperUserProfile
    {

        #region Consts
        /// <summary>
        /// Occasional User: tracking very few data per week
        /// </summary>
        public const float NotActiveUserTuningProb = 0.12f;

        /// <summary>
        /// Quite active user profile: few posts per week and only few additional data tracked
        /// </summary>
        public const float AlmostActiveUserTuningProb = 0.18f;

        /// <summary>
        /// Active user profile :one post per day plus a lot of additional data tracked.
        /// </summary>
        public const float ActiveUserTuningProb = 0.44f;

        /// <summary>
        /// Very active user profile: more then one post per day plus almost all additional data tracked.
        /// </summary>
        public const float VeryActiveUserTuningProb = 0.64f;

        /// <summary>
        /// Number of days which a period si made up by
        /// </summary>
        public const ushort DaysPerPeriod = 7;

        #endregion


        #region Poperties

        /// <summary>
        /// Probability that the user will eprform a Post-related action
        /// </summary>
        public DbWrapperPostConfig PostConfig { get; set; } = new DbWrapperPostConfig();

        /// <summary>
        /// Period [Weeks] between two body measures (default = 4)
        /// </summary>
        public byte DefaultMeasuresPeriod { get; set; } = 4;

        /// <summary>
        /// Period [Weeks] between two body measures (which might be randomicly changed accordint to the OffestMin/Max parameters)
        /// </summary>
        public byte MeasuresPeriod { get; set; }


        /// <summary>
        /// Minimum allowed offset in the Measures Period (randomicly chosen)
        /// </summary>
        public byte MeasuresPeriodOffsetMin { get; set; } = 2;

        /// <summary>
        /// Minimum allowed offset in the Measures Period (randomicly chosen)
        /// </summary>
        public byte MeasuresPeriodOffsetMax { get; set; } = 2;

        /// <summary>
        /// Date which the uer took measures last time
        /// </summary>
        public DateTime MeasuresLastDate { get; private set; }

        /// <summary>
        /// Default duration [Weeks] of the usual Diet Plan (default is 4)
        /// </summary>
        public byte DefaultDietPeriod { get; set; } = 4;

        /// <summary>
        /// Duration [Weeks] of the usual Diet Plan (which might be randomicly changed accordint to the OffestMin/Max parameters)
        /// </summary>
        public byte DietPeriod { get; set; }

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte DietPeriodOffsetMin { get; set; } = 2;

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte DietPeriodOffsetMax { get; set; } = 2;

        /// <summary>
        /// Date which the uer took measures last time
        /// </summary>
        public DateTime DietPlanStarted { get; private set; }

        /// <summary>
        /// Default duration [Weeks] of User Phase perdiods (default = 36)
        /// </summary>
        public byte DefaultPhasePeriod { get; set; } = 36;

        /// <summary>
        /// Duration [Weeks] of the usual Diet Plan (which might be randomicly changed accordint to the OffestMin/Max parameters)
        /// </summary>
        public byte PhasePeriod { get; set; }

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte PhasePeriodOffsetMin { get; set; } = 8;

        /// <summary>
        /// Minimum allowed offset in the Diet Period (randomicly chosen)
        /// </summary>
        public byte PhasePeriodOffsetMax { get; set; } = 12;

        /// <summary>
        /// Date which the uer took measures last time
        /// </summary>
        public DateTime PhaseStarted { get; private set; }


        /// <summary>
        /// Nominal user weight.Actual values will be randomly generated in very close range.
        /// </summary>
        public ushort Weight { get; set; } = 0;

        /// <summary>
        /// Training Data
        /// </summary>
        public DbWrapperTrainingProfile Training { get; set; } = null;

        #endregion


        #region Ctors
        /// <summary>
        /// Builds the probability of the actions an user will perform according to its activity degree.
        /// </summary>
        /// <param name="userActivityLevel">User activity degree in terms of probability (0 = NotActive, 1 = VeryActive)</param>
        public DbWrapperUserProfile(float userActivityLevel = 0.5f)
        {
            // Config
            PostConfig.PostsPerDay += (float)Math.Min(1.7, 1.9 * userActivityLevel);            // More than one post per day allowed
            PostConfig.DietDayProbability += userActivityLevel;
            PostConfig.ActivityDayProbability += userActivityLevel;
            PostConfig.WeightProbability += userActivityLevel;
            PostConfig.WellnessProbability += userActivityLevel;

            MeasuresPeriod = DefaultMeasuresPeriod;
            DietPeriod = DefaultDietPeriod;
            PhasePeriod = DefaultPhasePeriod;

            PhaseStarted = DateTime.MinValue;
            DietPlanStarted = DateTime.MinValue;
            MeasuresLastDate = DateTime.MinValue;

            //// Init the training plan
            //Training = new DbWrapperTrainingProfile();
        }
        #endregion


        #region Methods

        /// <summary>
        /// Build the Weight property.
        /// </summary>
        /// <param name="inputWeight">The weight. If not specified, a random value in a very close range of the Weight property will be used.</param>
        public void BuildUserWeight(ushort inputWeight = 0)
        {
            float weightOffsetPercentage = 0.1f;

            if (inputWeight == 0)
                Weight = (ushort)RandomFieldGenerator.RandomInt(
                    (int)(Weight * (1 - weightOffsetPercentage)), (int)(Weight * (1 + weightOffsetPercentage)));

            else if (Weight == 0)
                Weight = (ushort)RandomFieldGenerator.RandomInt(300, 1000);
            else
                Weight = inputWeight;
        }


        /// <summary>
        /// Get the number of parent Posts the user is writing Today
        /// </summary>
        /// <returns>The number of Post entries</returns>
        public ushort GetTodayPostsNumber()
        {
            return (ushort)Math.Round(RandomFieldGenerator.RandomDouble(0.25f * PostConfig.PostsPerDay, 2 * PostConfig.PostsPerDay), 0);
        }

        /// <summary>
        /// Checks if it's time to track weight according to the user activity level.
        /// </summary>
        /// <returns>Ok / false</returns>
        public bool IsTrackingWeight()
        {
            return RandomFieldGenerator.RandomDouble(0, 1f) < PostConfig.WeightProbability;
        }


        public bool IsTrackingActivity()
        {
            return RandomFieldGenerator.RandomDouble(0, 1f) < PostConfig.ActivityDayProbability;
        }


        public bool IsTrackingDiet()
        {
            return RandomFieldGenerator.RandomDouble(0, 1f) < PostConfig.DietDayProbability;
        }


        public bool IsTrackingWellness()
        {
            return RandomFieldGenerator.RandomDouble(0, 1f) < PostConfig.WellnessProbability;
        }


        /// <summary>
        /// Tells wheter it's time to change the UserPhase.
        /// </summary>
        /// <param name="currentDate">Date to be checked</param>
        /// <returns></returns>
        public bool IsUserPhaseExpired(DateTime currentDate)
        {
            bool ret;

            ret = PhaseStarted.AddDays(DaysPerPeriod * PhasePeriod) < currentDate;

            if (ret)
            {
                PhaseStarted = currentDate;
                PhasePeriod = (byte)RandomFieldGenerator.RandomInt(
                    DefaultPhasePeriod - PhasePeriodOffsetMin, DefaultPhasePeriod + PhasePeriodOffsetMax + 1);
            }

            return ret;
        }


        /// <summary>
        /// Tells wheter it's time to track measures.
        /// </summary>
        /// <param name="currentDate">Date to be checked</param>
        /// <returns></returns>
        public bool IsMeasureTime(DateTime currentDate)
        {
            bool ret;

            ret = MeasuresLastDate.AddDays(DaysPerPeriod * MeasuresPeriod) < currentDate;

            if (ret)
            {
                MeasuresLastDate = currentDate;
                MeasuresPeriod = (byte)RandomFieldGenerator.RandomInt(
                    DefaultMeasuresPeriod - MeasuresPeriodOffsetMin, DefaultMeasuresPeriod + MeasuresPeriodOffsetMax + 1);
            }

            return ret;
        }


        /// <summary>
        /// Tells wheter it's time to update the Diet Plan.
        /// </summary>
        /// <param name="currentDate">Date to be checked</param>
        /// <returns></returns>
        public bool IsDietPlanExpired(DateTime currentDate)
        {
            bool ret;

            ret = DietPlanStarted.AddDays(DaysPerPeriod * DietPeriod) < currentDate;

            if (ret)
            {
                DietPlanStarted = currentDate;
                DietPeriod = (byte)RandomFieldGenerator.RandomInt(
                    DefaultDietPeriod - DietPeriodOffsetMin, DefaultDietPeriod + DietPeriodOffsetMax + 1);
            }

            return ret;
        }


        /// <summary>
        /// Tells wheter it's time to change the Training Plan.
        /// </summary>
        /// <param name="currentDate">Date to be checked</param>
        /// <returns></returns>
        public bool IsTrainingPlanExpired(DateTime currentDate)
        {
            return Training == null ? true : Training.IsExpired(currentDate);
        }
        #endregion



        #region Private Methods

        #endregion
    }

}
