using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class FitnessDayWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "FitnessDayEntry";
        #endregion



        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime FitnessDayDate { get; set; } =  DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// User which the day refers to. WARNING: used to test the denormalized version of the tables: might disappear from the final schema.
        /// </summary>
        public int UserId { get; set; } = 0;
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the FitnessDayEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public FitnessDayWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
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

                    case "Rating":

                        col.Value = RandomFieldGenerator.RandomInt(0, 5);
                        break;

                    case "DayDate":

                        if (FitnessDayDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                        {
                            col.Value = DatabaseUtility.GetUnixTimestamp(FitnessDayDate);
                            // Reset
                            FitnessDayDate = DatabaseUtility.UnixTimestampT0;
                        }
                        break;

                    case "UserId":

                        if (UserId == 0)
                            col.Value = null;
                        else
                        {
                            col.Value = UserId;
                            // Reset
                            UserId = 0;
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
        /// <param name="parentId">The ID of the FitnessDayEntry table which this table refers to</param>
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

                    case "Rating":

                        col.Value = RandomFieldGenerator.RandomInt(0, 5);
                        break;

                    case "DayDate":

                        if (FitnessDayDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                        {
                            col.Value = DatabaseUtility.GetUnixTimestamp(FitnessDayDate);
                            // Reset
                            FitnessDayDate = DatabaseUtility.UnixTimestampT0;
                        }
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
