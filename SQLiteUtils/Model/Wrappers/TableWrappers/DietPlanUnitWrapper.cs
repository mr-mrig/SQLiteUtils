using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class DietPlanUnitWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "DietPlanUnit";
        #endregion



        #region Private Fields
        private int _dietPlanIdMin;
        private int _dietPlanIdMax;
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
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public DietPlanUnitWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get Diet Plan id
            List<int> ids = DatabaseUtility.GetTableIds(connection, "DietPlan");
            _dietPlanIdMin = ids.Min();
            _dietPlanIdMax = ids.Max();
        }


        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public DietPlanUnitWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _dietPlanIdMin = userIdMin;
            _dietPlanIdMax = userIdMax;
        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long dietPlanId = 0)
        {
            int? tmpDate = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "StartDate":

                        if (StartDate.Ticks == 0)
                        {
                            tmpDate = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                            col.Value = tmpDate;
                        }
                        else
                        {
                            tmpDate = DatabaseUtility.GetUnixTimestamp(StartDate);
                            col.Value = tmpDate;
                            StartDate = DatabaseUtility.UnixTimestampT0;
                        }
                        break;

                    case "EndDate":

                        if (EndDate.Ticks == 0)
                        {
                            col.Value = RandomFieldGenerator.RandomUnixDate(
                                tmpDate.Value, DatabaseUtility.UnixTimestampOneWeekDelta, DatabaseUtility.UnixTimestampOneMonthDelta, 0.1f);
                        }
                        else
                            col.Value = EndDate;

                        break;

                    case "EndDatePlanned":

                        //col.Value = RandomFieldGenerator.RandomUnixDate(
                        //    tmpDate.Value, DatabaseUtility.UnixTimestampOneWeekDelta, DatabaseUtility.UnixTimestampOneMonthDelta, 0.5f);
                        col.Value = null;
                        break;

                    case "DietPlanId":

                        if (dietPlanId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_dietPlanIdMin, _dietPlanIdMax + 1);
                        else
                        {
                            col.Value = dietPlanId;
                            dietPlanId = 0;
                        }
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

            StartDate = DatabaseUtility.UnixTimestampT0;
            EndDate = DatabaseUtility.UnixTimestampT0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
