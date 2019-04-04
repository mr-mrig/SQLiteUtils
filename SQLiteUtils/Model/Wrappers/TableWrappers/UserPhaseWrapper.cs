using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class UserPhaseWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "UserPhase";
        #endregion


        #region Private Fields
        #endregion


        #region Properties
        public int UserIdMin { get; set; }
        public int UserIdMax { get; set; }
        public int PhaseIdMin { get; set; }
        public int PhaseIdMax { get; set; }
        public int PhaseNoteIdMin { get; set; }
        public int PhaseNoteIdMax { get; set; }
        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime StartDate { get; set; } = DatabaseUtility.UnixTimestampT0;

        /// <summary>
        /// Specific date for the entry, random otherwise.
        /// </summary>
        public DateTime EndDate { get; set; } = DatabaseUtility.UnixTimestampT0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public UserPhaseWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;
            List<int> ids;

            tableName = "User";
            ids = DatabaseUtility.GetTableIds(connection, tableName);

            UserIdMin = ids.Min();
            UserIdMin = ids.Max();

            try
            {
                tableName = "Phase";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                PhaseIdMin = ids.Min();
                PhaseIdMax = ids.Max();

                tableName = "UserPhaseNote";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                PhaseNoteIdMin = ids.Min();
                PhaseNoteIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException($"{GetType().Name} - Table {tableName} has no rows");
            }
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userIdMin">Lowest userId ffrom the User table</param>
        /// <param name="userIdMax">Highest userId ffrom the User table</param>
        public UserPhaseWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            // Get User Ids
            UserIdMin = userIdMin;
            UserIdMax = userIdMax;

            List<int> ids = DatabaseUtility.GetTableIds(connection, "Phase");
            PhaseIdMin = ids.Min();
            PhaseIdMax = ids.Max();

            // Get UserPhaseNote Ids
            ids = DatabaseUtility.GetTableIds(connection, "UserPhaseNote");
            PhaseNoteIdMin = ids.Min();
            PhaseNoteIdMax = ids.Max();
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long userId = 0)
        {
            int date1 = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "StartDate":

                        if (StartDate.Ticks == 0)
                            date1 = RandomFieldGenerator.RandomUnixDate(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).Value;

                        else
                            col.Value = DatabaseUtility.GetUnixTimestamp(StartDate);

                        date1 = (int)col.Value;
                        break;

                    case "EndDate":

                        if (EndDate.Ticks == 0)
                            col.Value = RandomFieldGenerator.RandomUnixDate(date1, DatabaseUtility.UnixTimestampOneMonthDelta, DatabaseUtility.UnixTimestampSixMonthsDelta);

                        else
                            col.Value = DatabaseUtility.GetUnixTimestamp(EndDate);

                        break;

                    case "CreatedOn":

                        col.Value = date1;
                        break;

                    case "UserPhaseNoteId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(PhaseNoteIdMin, PhaseNoteIdMax + 1, 0.5f);
                        break;

                    case "PhaseId":

                        col.Value = RandomFieldGenerator.RandomInt(PhaseIdMin, PhaseIdMax + 1);
                        break;

                    case "OwnerId":

                        if (userId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(UserIdMin, UserIdMax + 1);
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
