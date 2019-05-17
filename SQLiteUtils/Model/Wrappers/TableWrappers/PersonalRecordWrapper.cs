using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class PersonalRecordWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "PersonalRecord";
        #endregion


        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        private int _excerciseIdMin;
        private int _excerciseIdMax;
        #endregion


        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime RecordDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// Specific User Id for the entry, random otherwise.
        /// </summary>
        public long UserId { get; set; } = 0;

        /// <summary>
        /// Excercise Id for the entry, random otherwise.
        /// </summary>
        public long ExcerciseId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public PersonalRecordWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, true)
        {
            _userIdMin = 1;
            _userIdMax = DatabaseUtility.GetTableMaxId(connection, "User", true);

            _excerciseIdMin = 1;
            _excerciseIdMax = DatabaseUtility.GetTableMaxId(connection, "Excercise", true);
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public PersonalRecordWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName, true)
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


                    case "Value":

                        col.Value = RandomFieldGenerator.RandomInt(50, 1200);
                        break;

                    case "RecordDate":

                        if (RecordDate == DatabaseUtility.UnixTimestampT0)
                            col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                            col.Value = DatabaseUtility.GetUnixTimestamp(RecordDate);
                        break;

                    case "ExcerciseId":

                        if (ExcerciseId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_excerciseIdMin, _excerciseIdMax + 1);
                        else
                            col.Value = ExcerciseId;

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
            RecordDate = DatabaseUtility.UnixTimestampT0;
            UserId = 0;
            ExcerciseId = 0;

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
