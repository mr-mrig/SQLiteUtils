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
        public WorkUnitWrapper(SQLiteConnection connection) : base(connection, DefaultTableName)
        {
            List<int> ids = DatabaseUtility.GetTableIds(connection, "WorkoutSession");
            _workoutIdMin = ids.Min();
            _workoutIdMax = ids.Max();

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
        public override List<DatabaseColumnWrapper> GenerateRandomEntry(long workoutId = 0)
        {

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

                        col.Value = RandomFieldGenerator.RandomInt(_exerciseIdMin, _exerciseIdMax + 1);
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

            OrderNumber = 0;

            // New entry processed
            GeneratedEntryNumber++;

            return Entry;
        }
        #endregion
    }
}
