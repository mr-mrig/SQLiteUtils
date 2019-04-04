using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class CircumferenceWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "Circumference";
        #endregion



        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctors
        /// <summary>
        /// Wrapper for the Circumference DB table.
        /// </summary>
        /// <param name="connection"></param>
        public CircumferenceWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            List<int> ids = DatabaseUtility.GetTableIds(connection, "User");

            _userIdMin = ids.Min();
            _userIdMax = ids.Max();
        }



        /// <summary>
        /// Wrapper for the Circumference DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public CircumferenceWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
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

                    case "Neck":

                        col.Value = RandomFieldGenerator.RandomInt(200, 500);
                        break;

                    case "Chest":

                        col.Value = RandomFieldGenerator.RandomInt(700, 1800);
                        break;

                    case "Shoulders":

                        col.Value = RandomFieldGenerator.RandomInt(600, 1200);
                        break;

                    case "LeftForearm":

                        col.Value = RandomFieldGenerator.RandomInt(80, 400);
                        break;

                    case "RightForearm":

                        col.Value = RandomFieldGenerator.RandomInt(80, 400);
                        break;

                    case "LeftArm":

                        col.Value = RandomFieldGenerator.RandomInt(150, 600);
                        break;

                    case "RightArm":

                        col.Value = RandomFieldGenerator.RandomInt(150, 600);
                        break;

                    case "Waist":

                        col.Value = RandomFieldGenerator.RandomInt(300, 1200);
                        break;

                    case "Hips":

                        col.Value = RandomFieldGenerator.RandomInt(500, 1500);
                        break;

                    case "OwnerId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_userIdMin, _userIdMax, 0.5f);
                        break;

                    case "LeftLeg":

                        col.Value = RandomFieldGenerator.RandomInt(250, 1000);
                        break;

                    case "RightLeg":

                        col.Value = RandomFieldGenerator.RandomInt(250, 1000);
                        break;

                    case "LeftCalf":

                        col.Value = RandomFieldGenerator.RandomInt(250, 1000);
                        break;

                    case "RightCalf":

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
