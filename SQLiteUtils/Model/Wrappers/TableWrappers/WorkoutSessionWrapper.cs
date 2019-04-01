using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WorkoutSessionWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "WorkoutSession";
        #endregion


        #region Private Fields
        private int _weekIdMin { get; set; }
        private int _weekIdMax { get; set; }
        private int _lastPlanId = 0;
        private int _lastProgressiveNumber = 0;
        #endregion


        #region Properties
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WorkoutSessionWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingWeek");
            _weekIdMin = ids.Min();
            _weekIdMax = ids.Max();

        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long weekId = 0)
        {
            int tempTs = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(5, 20, 0.05f);
                        break;

                    case "Rating":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.3f);

                        break;


                    case "StartTime":

                        if (StartTime != DateTime.MinValue)
                            col.Value = DatabaseUtility.GetUnixTimestamp(StartTime);
                        else
                            col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);

                        tempTs = (int)col.Value;
                        break;

                    case "EndTime":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(tempTs, DatabaseUtility.UnixTimestampOneHourDelta, DatabaseUtility.UnixTimestampThreeHoursDelta, 0.1f);
                        break;


                    case "PlannedDate":

                        col.Value = null;
                        break;


                    case "TrainingWeekId":

                        if (weekId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_weekIdMin, _weekIdMax + 1);
                        else
                            col.Value = weekId;

                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }

            // Create new ID
            try
            {
                checked { MaxId++; };
            }
            catch (OverflowException)
            {
                return null;
            }


            StartTime = DateTime.MinValue;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
