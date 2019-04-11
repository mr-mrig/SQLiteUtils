using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils.Model
{
    public class WorkUnitWrapper : DatabaseObjectWrapper
    {


        #region Consts
        private const string DefaultTableName = "WorkUnit";
        #endregion


        #region Private Fields
        private int _workoutIdMin;
        private int _workoutIdMax;
        private int _exerciseIdMin;
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

        private int _exerciseIdMax;

        /// <summary>
        /// Excercise maximum Id. Stored here as there is no wrapper for the excercise table.
        /// </summary>
        public int ExcerciseMaxId
        {
            get => _exerciseIdMax;
            set => _exerciseIdMax = value;
        }
        #endregion


        #region Ctor
        /// <summary>
        /// Wrapper for the Post DB table.
        /// </summary>
        /// <param name="connection"></param>
        public WorkUnitWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            string tableName = string.Empty;
            List<int> ids;

            tableName = "WorkoutSession";
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

            // Parse columns and generate the fields
            foreach (DatabaseColumnWrapper col in Entry)
            {
                switch (col.Name)
                {


                    case "ProgressiveNumber":

                        col.Value = OrderNumber;
                        break;

                    case "Comment":

                        col.Value = RandomFieldGenerator.RandomTextValue(10, 250, 0.8f);
                        break;

                    case "QuickRating":

                        col.Value = RandomFieldGenerator.RandomIntNullable(0, 5, 0.2f);
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

                    case "WorkoutSessionId":

                        if (workoutId == 0)
                            col.Value = RandomFieldGenerator.RandomInt(_workoutIdMin, _workoutIdMax + 1);
                        else
                            col.Value = workoutId;

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
