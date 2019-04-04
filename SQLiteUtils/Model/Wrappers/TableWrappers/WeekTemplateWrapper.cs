using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WeekTemplateWrapper : DatabaseObjectWrapper
    {


        #region Enum
        public enum WeekType : byte
        {
            Deload = 1,
            Peak,
            Tapering,
            None
        }
        #endregion


        #region Consts
        private const string DefaultTableName = "TrainingWeekTemplate";
        #endregion


        #region Private Fields
        private int _planIdMin { get; set; }
        private int _planIdMax { get; set; }
        private int _weekIdMin { get; set; }
        private int _weekIdMax { get; set; }
        private int _lastPlanId = 0;
        private int _lastProgressiveNumber = 0;
        #endregion


        #region Properties
        public int OrderNumber { get; set; } = 0;
        public WeekType WeekTypeId { get; set; } = WeekType.None;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WeekTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingPlan");
            _planIdMin = ids.Min();
            _planIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "TrainingWeekType");
            _weekIdMin = ids.Min();
            _weekIdMax = ids.Max();
        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long planId = 0)
        {
            long currentPlanId = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "ProgressiveNumber":


                        //if (currentPlanId == _lastPlanId)
                        //    _lastProgressiveNumber++;
                        //else
                        //    _lastProgressiveNumber = 0;

                        col.Value = OrderNumber;
                        OrderNumber = 0;

                        break;

                    case "TrainingWeekTypeId":

                        if (WeekTypeId == WeekType.None)
                            col.Value = null;
                        else
                            col.Value = RandomFieldGenerator.RandomInt(_weekIdMin, _weekIdMax + 1);

                        break;

                    case "TrainingPlanId":

                        if (planId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_planIdMin, _planIdMax + 1);
                        else
                            col.Value = planId;

                        currentPlanId = (long)col.Value;
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

            WeekTypeId = WeekType.None;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
