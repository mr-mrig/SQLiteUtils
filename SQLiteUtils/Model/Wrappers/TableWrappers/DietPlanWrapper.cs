using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class DietPlanWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "DietPlan";
        #endregion



        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        #endregion


        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime CreatedOnDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// Specific User Id for the entry, random otherwise.
        /// </summary>
        public int OwnerId { get; set; } = 0;
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public DietPlanWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            _userIdMin = 1;
            _userIdMax = DatabaseUtility.GetTableMaxId(connection, "User", true);
        }


        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public DietPlanWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;
        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {

                    case "Id":

                        col.Value = parentId;
                        break;

                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(5, 20));
                        break;

                    case "WeeklyFreeMealsNumber":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 7, 0.3f);
                        break;

                    case "CreatedOn":

                        if (CreatedOnDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                        {
                            col.Value = DatabaseUtility.GetUnixTimestamp(CreatedOnDate);
                            // Reset
                            CreatedOnDate = DatabaseUtility.UnixTimestampT0;
                        }
                        break;

                    case "LastUpdate":

                        col.Value = null;
                        break;

                    case "OwnerId":

                        if(OwnerId == 0)
                            col.Value = RandomFieldGenerator.RandomIntNullable(_userIdMin, _userIdMax, 0.7f);
                        else
                        {
                            col.Value = OwnerId;
                            OwnerId = 0;
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
                checked
                {
                    MaxId = parentId > 0 ? parentId : MaxId + 1;
                };
            }
            catch (OverflowException)
            {
                return null;
            }

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public List<DatabaseColumnWrapper> GenerateRandomEntry()
        {

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

                    case "Id":

                        throw new NotImplementedException();
                        break;

                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(5, 20));
                        break;

                    case "WeeklyFreeMealsNumber":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 7, 0.3f);
                        break;

                    case "CreatedOn":

                        if (CreatedOnDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                        {
                            col.Value = DatabaseUtility.GetUnixTimestamp(CreatedOnDate);
                            // Reset
                            CreatedOnDate = DatabaseUtility.UnixTimestampT0;
                        }
                        break;

                    case "LastUpdate":

                        //if (CreatedOnDate.Ticks == 0)
                        //    col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound, 0.7f);
                        //else
                        //{
                        //    col.Value = DatabaseUtility.GetUnixTimestamp(CreatedOnDate);
                        //    // Reset
                        //    CreatedOnDate = DatabaseUtility.UnixTimestampT0;
                        //}
                        break;

                    case "OwnerId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_userIdMin, _userIdMax, 0.7f);
                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }
            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
