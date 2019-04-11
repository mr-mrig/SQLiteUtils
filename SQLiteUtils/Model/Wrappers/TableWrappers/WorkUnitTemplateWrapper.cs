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

        /// <summary>
        /// Work unit progressive number
        /// </summary>
        public int OrderNumber { get; set; } = 0;

        /// <summary>
        /// Excecise Id
        /// </summary>
        public int ExcerciseId { get; set; } = 0;
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WorkUnitTemplateWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;
            List<int> ids;

            tableName = "WorkoutTemplate";
            ids = DatabaseUtility.GetTableIds(connection, tableName);

            try
            {
                _workoutIdMin = ids.Min();
                _workoutIdMax = ids.Max();
            }
            catch
            {
                _workoutIdMin = 0;
                _workoutIdMax = 0;
            }
            try
            {
                tableName = "WorkUnitTemplateNote";
                ids = DatabaseUtility.GetTableIds(connection, tableName);

                _wuNoteIdMin = ids.Min();
                _wuNoteIdMax = ids.Max();
            }
            catch
            {
                _wuNoteIdMin = 0;
                _wuNoteIdMax = 0;
            }
            try
            {
                tableName = "Excercise";
                ids = DatabaseUtility.GetTableIds(connection, tableName);
                _exerciseIdMin = ids.Min();
                _exerciseIdMax = ids.Max();
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
        public override List<DatabaseColumnWrapper> Create(long workoutId = 0)
        {
            long currentPlanId = 0;

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

                        if (ExcerciseId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_exerciseIdMin, _exerciseIdMax + 1);
                        else
                        {
                            col.Value = ExcerciseId;
                            ExcerciseId = 0;
                        }
                        break;

                    case "WorkUnitTemplateNoteId":

                        col.Value = RandomFieldGenerator.RandomIntNullable(_wuNoteIdMin, _wuNoteIdMax + 1, 0.7f);
                        break;

                    case "WorkoutTemplateId":

                        if (workoutId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_workoutIdMin, _workoutIdMax + 1);
                        else
                            col.Value = workoutId;

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
