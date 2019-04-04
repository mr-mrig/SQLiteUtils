using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WorkoutTemplateWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "WorkoutTemplate";
        #endregion


        #region Private Fields
        private int _weekIdMin { get; set; }
        private int _weekIdMax { get; set; }
        #endregion


        #region Properties
        public int OrderNumber { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WorkoutTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "TrainingWeekTemplate");
            _weekIdMin = ids.Min();
            _weekIdMax = ids.Max();

        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long weekId = 0)
        {
            long currentPlanId = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "Name":

                        col.Value = RandomFieldGenerator.RandomTextValue(5, 20, 0.5f);

                        break;

                    case "ProgressiveNumber":

                        col.Value = OrderNumber;
                        OrderNumber = 0;

                        break;

                    case "IsWeekDaySpecific":

                        col.Value = RandomFieldGenerator.RandomInt(0, 2);
                        break;

                    case "TrainingWeekId":

                        if (weekId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_weekIdMin, _weekIdMax + 1);
                        else
                            col.Value = weekId;

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



            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
