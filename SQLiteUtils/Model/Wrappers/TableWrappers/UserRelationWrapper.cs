using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model
{
    public class UserRelationWrapper : DatabaseObjectWrapper
    {



        #region Consts
        private const string DefaultTableName = "UserRelation";
        private const string tableTemplate = "user";
        #endregion


        #region Private Fields
        private int _userIdMin;
        private int _userIdMax;
        private int _relationStatusIdMin;
        private int _relationStatusIdMax;
        #endregion



        #region Ctors
        public UserRelationWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, false)
        {
            // Get User Ids
            List<int> userIds = DatabaseUtility.GetTableIds(connection, "User");
            _userIdMin = userIds.Min();
            _userIdMax = userIds.Max();

            // Get RelationStatus Ids
            List<int> RelationStatusIds = DatabaseUtility.GetTableIds(connection, "RelationStatus");
            _relationStatusIdMin = RelationStatusIds.Min();
            _relationStatusIdMax = RelationStatusIds.Max();
        }

        public UserRelationWrapper(SQLiteConnection connection, int userIdMin, int userIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;

            // Get RelationStatus Ids
            List<int> RelationStatusIds = DatabaseUtility.GetTableIds(connection, "RelationStatus");
            _relationStatusIdMin = RelationStatusIds.Min();
            _relationStatusIdMax = RelationStatusIds.Max();
        }

        public UserRelationWrapper(SQLiteConnection connection, int userIdMin, int userIdMax, int relationTypeIdMin, int relationTypeIdMax) : base(connection, DefaultTableName)
        {
            _userIdMin = userIdMin;
            _userIdMax = userIdMax;

            _relationStatusIdMin = relationTypeIdMin;
            _relationStatusIdMax = relationTypeIdMax;
        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="sourceUserId">SourceUserId, random if not specified</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long sourceUserId = 0)
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
                    case "SourceUserId":

                        // Store the Source user to ensure if doesn't appear as Target also
                        if (sourceUserId == 0)
                        {
                            sourceUserId = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax + 1);
                            col.Value = sourceUserId;
                        }
                        else
                            col.Value = sourceUserId;

                        break;

                    case "TargetUserId":

                        col.Value = RandomFieldGenerator.RandomIntValueExcluded(_userIdMin, _userIdMax, new List<int> { (int)sourceUserId });
                        break;

                    case "LastUpdate":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound, 0.5f);
                        break;


                    case "RelationStatusId":

                        col.Value = RandomFieldGenerator.RandomInt(_relationStatusIdMin, _relationStatusIdMax + 1);
                        break;

                    default:

                        col.Value = RandomFieldGenerator.GenerateRandomField(col);
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
