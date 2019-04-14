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
        public long UserId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public PostWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, true)
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
        public PostWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName, true)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="parentId">Id, otherwise it will be automatically-generated</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {

                    case "Id":

                        col.Value = parentId;
                        break;

                    case "Caption":

                        col.Value = RandomFieldGenerator.RandomTextValue(RandomFieldGenerator.RandomInt(50, 500));
                        break;

                    case "CreatedOn":

                        if (CreatedOnDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                            col.Value = DatabaseUtility.GetUnixTimestamp(CreatedOnDate);
                        break;

                    case "IsPublic":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "LastUpdate":

                        col.Value = null;
                        break;

                    case "UserId":

                        if (UserId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax + 1);
                        else
                            col.Value = UserId;

                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
                        break;
                }
            }
            CreatedOnDate = DatabaseUtility.UnixTimestampT0;

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
