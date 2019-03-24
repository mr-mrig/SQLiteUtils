using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingScheduleWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingSchedule";
        #endregion


        #region Private Fields
        private int _proficiencyIdMin;
        private int _poficiencyIdMax;
        private int _planIdMin;
        private int _planIdMax;
        private int _phaseIdMin;
        private int _phaseIdMax;
        #endregion


        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime StartDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime EndDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// FK
        /// </summary>
        public int CurrentWeekId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingScheduleWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingProficiency");
            _proficiencyIdMin = ids.Min();
            _poficiencyIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "TrainingPlan");
            _planIdMin = ids.Min();
            _planIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "Phase");
            _phaseIdMin = ids.Min();
            _phaseIdMax = ids.Max();
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="planId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long planId = 0)
        {
            int tempTs = 0;

            // Create new ID
            try
            {
                checked { MaxId++; };
            }
            catch (OverflowException)
            {
                return null;
            }


            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "PlannedEndDate":

                        if (RandomFieldGenerator.RandomDouble(0, 1) < 0.1)
                            col.Value = tempTs;
                        else
                            col.Value = null;
                        break;


                    case "EndDate":

                        if (EndDate != DatabaseUtility.UnixTimestampT0)
                            col.Value = EndDate;
                        else
                            col.Value = RandomFieldGenerator.RandomUnixDate(tempTs, DatabaseUtility.UnixTimestampOneWeekDelta, DatabaseUtility.UnixTimestampThreeMonthsDelta);

                        tempTs = (int)col.Value;
                        break;


                    case "StartDate":

                        if (StartDate != DatabaseUtility.UnixTimestampT0)
                            col.Value = StartDate;
                        else
                            col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);

                        tempTs = (int)col.Value;
                        break;

                    case "PhaseId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_phaseIdMin, _phaseIdMax + 1, 0.1f);
                        break;

                    case "CurrentWeekId":

                        if (CurrentWeekId == 0)
                            col.Value = null;
                        else
                            col.Value = CurrentWeekId;
                        break;

                    case "TrainingProficiencyId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_proficiencyIdMin, _poficiencyIdMax + 1, 0.1f);
                        break;

                    case "TrainingPlanId":

                        if (planId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_planIdMin, _planIdMax + 1);
                        else
                            col.Value = planId;

                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }

            StartDate = DatabaseUtility.UnixTimestampT0;
            EndDate = DatabaseUtility.UnixTimestampT0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
