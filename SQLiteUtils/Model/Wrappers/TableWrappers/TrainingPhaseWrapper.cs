using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingPhaseWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingPlanPhase";
        #endregion


        #region Private Fields
        private int _planIdMin;
        private int _planIdMax;
        private int _phaseIdMin;
        private int _phaseIdMax;
        #endregion


        #region Properties

        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingPhaseWrapper(SQLiteConnection connection) : base(connection, DefaultTableName, false)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingPlan");
            _planIdMin = ids.Min();
            _planIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "Phase");
            _phaseIdMin = ids.Min();
            _phaseIdMax = ids.Max();
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long planId = 0)
        {
            int date1 = 0;


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


                    case "PlanId":

                        if (planId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_planIdMin, _planIdMax + 1);

                        else
                            col.Value = planId;

                        break;

                    case "PhaseId":

                        col.Value = RandomFieldGenerator.RandomInt(_phaseIdMin, _phaseIdMax + 1);
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
