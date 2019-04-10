using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingPlanWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingPlan";
        #endregion


        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        private int _noteIdMin;
        private int _noteIdMax;
        #endregion


        #region Properties
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime CreatedOnDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// FK
        /// </summary>
        public int TrainingPlanNoteId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingPlanWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;

            tableName = "User";
            List<int> ids = DatabaseUtility.GetTableIds(connection, tableName);

            try
            {
                _userIdMin = ids.Min();
                _userIdMax = ids.Max();
            }
            catch
            {
                _userIdMin = 0;
                _userIdMax = 0;
            }

            try
            {
                tableName = "TrainingPlanNote";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                _noteIdMin = ids.Min();
                _noteIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException($"{this.GetType().Name} - Table {tableName} has no rows");
            }
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId from the User table</param>
        /// <param name="userIdMax">Highest userId from the User table</param>
        /// <param name="noteIdMin">Lowest noteId from the TrainingPlanNote table</param>
        /// <param name="noteIdMax">Highest noteId from the TrainingPlanNote table</param>
        public TrainingPlanWrapper(SQLiteConnection connection, int userIdMin, int userIdMax, int noteIdMin, int noteIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;

            _noteIdMin = noteIdMin;
            _noteIdMax = noteIdMax;
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId from the User table</param>
        /// <param name="userIdMax">Highest userId from the User table</param>

        public TrainingPlanWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;

            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingPlanNote");
            try
            {
                _noteIdMin = ids.Min();
                _noteIdMax = ids.Max();
            }
            catch
            {
                _noteIdMin = 0;
                _noteIdMax = 0;
            }
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


                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(10, 25);
                        break;

                    case "Description":

                        col.Value = RandomFieldGenerator.RandomTextValue(50, 500, 0.4f);
                        break;

                    case "CreatedOn":

                        if (CreatedOnDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        else
                            col.Value = DatabaseUtility.GetUnixTimestamp(CreatedOnDate);
                        break;

                    case "IsBookmarked":
                    case "IsTemplate":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "LastUpdate":

                        col.Value = null;
                        break;

                    case "OwnerId":

                        if (userId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax + 1);
                        else
                            col.Value = userId;

                        break;

                    case "TrainingPlanNoteId":

                        if (TrainingPlanNoteId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_noteIdMin, _noteIdMax + 1);
                        else
                            col.Value = TrainingPlanNoteId;

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
