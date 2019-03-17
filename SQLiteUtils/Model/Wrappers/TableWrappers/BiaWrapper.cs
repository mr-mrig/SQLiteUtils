using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class BiaWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "BiaEntry";
        #endregion



        #region Private Fields
        private int _biaDeviceIdMin;
        private int _biaDeviceIdMax;
        private int _userIdMin;
        private int _userIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        public BiaWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            // Get User Ids
            List<int> ids = DatabaseUtility.GetTableIds(connection, "User");
            _userIdMin = ids.Min();
            _userIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "BiaDevice");
            _biaDeviceIdMin = ids.Min();
            _biaDeviceIdMax = ids.Max();
        }


        /// <summary>
        /// Wrapper for the BiaEntry DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public BiaWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;
        }
        #endregion



        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long parentId)
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

                    case "Kg":

                        col.Value = RandomFieldGenerator.RandomInt(300, 1200);
                        break;

                    case "Bf":

                        col.Value = RandomFieldGenerator.RandomInt(30, 400);
                        break;

                    case "Tbw":

                        col.Value = RandomFieldGenerator.RandomInt(500, 750);
                        break;

                    case "Ecw":

                        col.Value = RandomFieldGenerator.RandomInt(350, 600);
                        break;

                    case "EcMatrix":

                        col.Value = RandomFieldGenerator.RandomInt(80, 500);
                        break;

                    case "Bmr":

                        col.Value = RandomFieldGenerator.RandomInt(800, 4000);
                        break;

                    case "Hpa":

                        col.Value = RandomFieldGenerator.RandomInt(20, 200);
                        break;

                    case "BiaDeviceId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_biaDeviceIdMin, _biaDeviceIdMax, 0.7f);
                        break;

                    case "OwnerId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_userIdMin, _userIdMax, 0.7f);
                        break;

                    default:

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
