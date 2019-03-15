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
        public UserRelationWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            // Get User Ids
            List<int> userIds = DatabaseUtility.GetTableIds(connection, "User");
            int userIdsMin = userIds.Min();
            int userIdsMax = userIds.Max();

            // Get RelationStatus Ids
            List<int> RelationStatusIds = DatabaseUtility.GetTableIds(connection, "RelationStatus");
            int _relationStatusIdMin = RelationStatusIds.Min();
            int _relationStatusIdMax = RelationStatusIds.Max();
        }
        #endregion


        #region  Override Methods

        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long parentId = 0)
        {
            int sourceUserId = 1;

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
                        sourceUserId = RandomFieldGenerator.RandomInt(_userIdMin, _userIdMax);
                        col.Value = sourceUserId.ToString();
                        break;

                    case "TargetUserId":

                        col.Value = RandomFieldGenerator.RandomIntValueExcluded(_userIdMin, _userIdMax, new List<int> { sourceUserId }).ToString();
                        break;

                    case "LastUpdate":

                        col.Value = RandomFieldGenerator.RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound).ToString();
                        break;


                    case "RelationStatusId":

                        col.Value = new Random().Next(_relationStatusIdMin, _relationStatusIdMax).ToString();
                        break;

                    default:
                        if (col.ValType == null)
                            return null;
                        else
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
