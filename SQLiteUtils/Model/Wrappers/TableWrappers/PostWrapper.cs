using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class PostWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "Post";
        private const string tableTemplate = "post";
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
        public int UserId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public PostWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            List<int> ids = DatabaseUtility.GetTableIds(connection, "User");

            _userIdMin = ids.Min();
            _userIdMax = ids.Max();
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public PostWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long userId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "Caption":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(50, 500));
                        break;

                    case "CreatedOn":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        break;

                    case "IsPublic":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "LastUpdate":

                        col.Value = null;
                        break;

                    case "UserId":

                        if (userId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax + 1);
                        else
                            col.Value = userId;

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
