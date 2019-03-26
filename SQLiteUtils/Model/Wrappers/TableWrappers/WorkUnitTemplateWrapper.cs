using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WorkUnitTemplateWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "WorkUnitTemplate";
        #endregion


        #region Private Fields
        private int _workoutIdMin;
        private int _workoutIdMax;
        private int _wuNoteIdMin;
        private int _wuNoteIdMax;
        private int _exerciseIdMin;
        private int _exerciseIdMax;
        #endregion


        #region Properties
        public int OrderNumber { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WorkUnitTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "WorkoutTemplate");
            _workoutIdMin = ids.Min();
            _workoutIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "WorkUnitTemplateNote");
            _wuNoteIdMin = ids.Min();
            _wuNoteIdMax = ids.Max();

            ids = DatabaseUtility.GetTableIds(connection, "Excercise");
            _exerciseIdMin = ids.Min();
            _exerciseIdMax = ids.Max();

        }
        #endregion


        #region Override Methods
        /// <summary>
        /// Generates an entry with random but meaningful values. DB Integreity is ensured.
        /// <param name="userId">User Id, otherwise it will be random</param>
        /// </summary>
        public override List<DatabaseColumnWrapper> Create(long workoutId = 0)
        {
            int currentPlanId = 0;

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "ProgressiveNumber":

                        col.Value = OrderNumber;
                        OrderNumber = 0;

                        break;

                    case "ExcerciseId":

                        col.Value = RandomFieldGenerator.RandomInt(_exerciseIdMin, _exerciseIdMax + 1);
                        break;

                    case "WorkUnitTemplateNoteId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_wuNoteIdMin, _wuNoteIdMax + 1, 0.7f);
                        break;

                    case "WorkoutTemplateId":

                        if (workoutId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_workoutIdMin, _workoutIdMax + 1);
                        else
                            col.Value = workoutId;

                        currentPlanId = (int)col.Value;
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
