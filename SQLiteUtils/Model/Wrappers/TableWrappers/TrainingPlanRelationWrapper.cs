using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingPlanRelationWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingPlanRelation";
        #endregion


        #region Private Fields
        private int _planIdMin;
        private int _planIdMax;
        private int _relationIdMin;
        private int _relationIdMax;
        private int _msgIdMin;
        private int _msgIdMax;
        #endregion


        #region Properties

        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingPlanRelationWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingPlan");
            _planIdMin = ids.Min();
            _planIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "TrainingPlanRelationType");
            _relationIdMin = ids.Min();
            _relationIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "TrainingPlanMessage");
            _msgIdMin = ids.Min();
            _msgIdMax = ids.Max();
        }


        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="planIdMin">Lowest noteId from the TrainingPlanNote table</param>
        /// <param name="planIdMax">Highest noteId from the TrainingPlanNote table</param>
        public TrainingPlanRelationWrapper(SQLiteConnection connection, int planIdMin, int planIdMax) : base(connection, DefaultTableName, false)
        {
            _planIdMin = planIdMin;
            _planIdMax = planIdMax;
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long parentPlanId = 0)
        {
            int id1 = 0;

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


                    case "ParentPlanId":

                        if (parentPlanId == 0)
                            id1 = RandomFieldGenerator.RandomInt(_planIdMin, _planIdMax + 1);
                        else
                            id1 = (int)parentPlanId;

                        col.Value = id1;
                        break;

                    case "ChildPlanId":

                        col.Value = RandomFieldGenerator.RandomIntValueExcluded(_planIdMin, _planIdMax + 1, new List<int>() { id1 });
                        break;

                    case "TrainingPlanRelationTypeId":

                        col.Value = RandomFieldGenerator.RandomInt(_relationIdMin, _relationIdMax + 1);
                        break;

                    case "TrainingPlanMessageId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_msgIdMin, _msgIdMax + 1, 0.6f);
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
