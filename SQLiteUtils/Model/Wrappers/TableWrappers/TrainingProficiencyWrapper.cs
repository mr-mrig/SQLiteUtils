using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingProficiencyWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingProficiency";
        #endregion


        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        #endregion


        #region Properties
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingProficiencyWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;

            tableName = "User";
            List<int> ids = DatabaseUtility.GetTableIds(connection, tableName);

            _userIdMin = ids.Min();
            _userIdMax = ids.Max();
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

                        col.Value = RandomFieldGenerator.RandomTextValue(10, 20);
                        break;

                    case "Description":

                        col.Value = RandomFieldGenerator.RandomTextValue(50, 500, 0.4f);
                        break;

                    case "CreatedOn":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                        break;

                    case "IsApproved":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "OwnerId":

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
