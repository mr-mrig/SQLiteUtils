using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingTargetProficiencyWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingPlanTarget";
        #endregion


        #region Private Fields
        private int _planIdMin { get; set; }
        private int _planIdMax { get; set; }
        private int _proficiencyIdMin { get; set; }
        private int _proficiencyIdMax { get; set; }
        #endregion


        #region Properties

        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingTargetProficiencyWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, false)
        {
            string tableName = string.Empty;
            List<int> ids;

            tableName = "TrainingPlan";
            ids = DatabaseUtility.GetTableIds(connection, tableName);

            _planIdMin = ids.Min();
            _planIdMax = ids.Max();

            try
            {
                tableName = "TrainingProficiency";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                _proficiencyIdMin = ids.Min();
                _proficiencyIdMax = ids.Max();
            }
            catch
            {
                throw new SQLiteException($"{GetType().Name} - Table {tableName} has no rows");
            }
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long planId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "TrainingProficiencyId":

                        col.Value = RandomFieldGenerator.RandomInt(_proficiencyIdMin, _proficiencyIdMax + 1);
                        break;

                    case "TrainingPlanId":

                        if (planId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_planIdMin, _planIdMax + 1);
                        else
                            col.Value = planId;

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
