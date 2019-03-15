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
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long parentId = 0)
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
                    case "Rating":

                        col.Value = $@"{RandomFieldGenerator.RandomInt(0, 5)}";
                        break;

                    case "Date":

                        col.Value = $@"{RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString()}";
                        break;

                    default:

                        if (col.ValType == null)
                            return null;
                        else
                            col.Value = RandomFieldGenerator.GenerateRandomField(col.Affinity);

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
