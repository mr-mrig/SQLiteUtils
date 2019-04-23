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
        private const string DefaultTableName = "TrainingPlanHasPhase";
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
            try
            {
                _planIdMin = 1;
                _planIdMax = DatabaseUtility.GetTableMaxId(connection, "TrainingPlan", true);
            }
            catch
            {
                _planIdMin = 0;
                _planIdMax = 0;
            }

            try
            {
                _phaseIdMin = 1;
                _phaseIdMax = DatabaseUtility.GetTableMaxId(connection, "Phase", true);
            }
            catch
            {
                throw new SQLiteException($"{GetType().Name} - Table Phase has no rows");
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
