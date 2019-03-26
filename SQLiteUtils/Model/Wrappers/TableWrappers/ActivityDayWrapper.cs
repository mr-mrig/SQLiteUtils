using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class ActivityDayWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "ActivityDay";
        #endregion




        #region Ctors
        public ActivityDayWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
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

                    case "Steps":

                        col.Value = RandomFieldGenerator.RandomIntNullable(4000, 30000, 0.05f);
                        break;

                    case "CaloriesOut":

                        col.Value = RandomFieldGenerator.RandomIntNullable(600, 9999, 0.05f);
                        break;

                    case "Stairs":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 500, 0.1f);
                        break;

                    case "SleepMinutes":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 800, 0.1f);
                        break;

                    case "SleepQuality":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.15f);
                        break;

                    case "HeartRateRest":

                        col.Value = RandomFieldGenerator.RandomIntNullable(20, 100, 0.15f);
                        break;

                    case "HeartRateMax":

                        col.Value = RandomFieldGenerator.RandomIntNullable(100, 220, 0.15f);
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

                    case "Steps":

                        col.Value = RandomFieldGenerator.RandomIntNullable(4000, 30000, 0.05f);
                        break;

                    case "CaloriesOut":

                        col.Value = RandomFieldGenerator.RandomIntNullable(600, 9999, 0.05f);
                        break;

                    case "Stairs":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 500, 0.1f);
                        break;

                    case "SleepMinutes":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 800, 0.1f);
                        break;

                    case "SleepQuality":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.15f);
                        break;

                    case "HeartRateRest":

                        col.Value = RandomFieldGenerator.RandomIntNullable(20, 100, 0.15f);
                        break;

                    case "HeartRateMax":

                        col.Value = RandomFieldGenerator.RandomIntNullable(100, 220, 0.15f);
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
