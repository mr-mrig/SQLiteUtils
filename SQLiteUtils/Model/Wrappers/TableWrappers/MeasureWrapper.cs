using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class MeasureWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "MeasuresEntry";
        #endregion



        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime MeasureDate { get; set; } = DatabaseUtility.UnixTimestampT0;
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the Measures DB table.
        /// </summary>
        /// <param name="connection"></param>
        public MeasureWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {

        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId)
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

                        col.Value = parentId;
                        break;

                    case "OwnerNote":

                        col.Value = RandomFieldGenerator.RandomTextValue(10, 250, 0.4f);
                        break;

                    case "Rating":

                        col.Value = RandomFieldGenerator.RandomInt(0, 5);
                        break;

                    case "MeasureDate":

                        if (MeasureDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                        {
                            col.Value = DatabaseUtility.GetUnixTimestamp(MeasureDate);
                            // Reset
                            MeasureDate = DatabaseUtility.UnixTimestampT0;
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
