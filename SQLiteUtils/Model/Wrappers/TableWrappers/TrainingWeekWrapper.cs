using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class TrainingWeekWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "TrainingWeek";
        #endregion


        #region Private Fields
        private int _scheduleIdMin;
        private int _scheduleIdMax;
        private int _weekIdMin;
        private int _weekIdMax;
        #endregion


        #region Properties
        public int OrderNumber { get; set; } = 0;
        public int NextWorkoutId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public TrainingWeekWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingSchedule");
            _scheduleIdMin = ids.Min();
            _scheduleIdMax = ids.Max();

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
        public override List<DatabaseColumnWrapper> Create(long scheduleId = 0)
        {

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "ProgressiveNumber":

                        col.Value = OrderNumber;
                        break;

                    case "NextWorkoutId":

                        if (NextWorkoutId != 0)
                            col.Value = NextWorkoutId;
                        else
                            col.Value = null;
                        break;

                    case "TrainingWeekTypeId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_weekIdMin, _weekIdMax + 1, 0.9f);
                        break;

                    case "TrainingScheduleId":

                        if (scheduleId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_scheduleIdMin, _scheduleIdMax + 1);
                        else
                            col.Value = scheduleId;

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


            OrderNumber = 0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
