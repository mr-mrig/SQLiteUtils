using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class PlicometryWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "Plicometry";
        #endregion



        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the Plicometry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public PlicometryWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            List<int> ids = DatabaseUtility.GetTableIds(connection, "User");
            try
            {
                _userIdMin = ids.Min();
                _userIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException("PlicometryWrapper - Table User has no rows");
            }
        }

        /// <summary>
        /// Wrapper for the Plicometry DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public PlicometryWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
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

                    case "Tricep":

                        col.Value = RandomFieldGenerator.RandomInt(20, 200);
                        break;

                    case "Chest":

                        col.Value = RandomFieldGenerator.RandomInt(20, 200);
                        break;

                    case "Armpit":

                        col.Value = RandomFieldGenerator.RandomInt(20, 150);
                        break;

                    case "Subscapular":

                        col.Value = RandomFieldGenerator.RandomInt(30, 250);
                        break;

                    case "Suprailiac":

                        col.Value = RandomFieldGenerator.RandomInt(35, 500);
                        break;

                    case "Abdomen":

                        col.Value = RandomFieldGenerator.RandomInt(35, 500);
                        break;

                    case "Thigh":

                        col.Value = RandomFieldGenerator.RandomInt(20, 300);
                        break;

                    case "OwnerId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_userIdMin, _userIdMax, 0.5f);
                        break;

                    case "Bf":

                        col.Value = RandomFieldGenerator.RandomInt(250, 1000);
                        break;

                    case "Kg":

                        col.Value = RandomFieldGenerator.RandomInt(250, 1000);
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
        #endregion
    }
}
