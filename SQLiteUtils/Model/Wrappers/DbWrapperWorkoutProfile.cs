using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SQLiteUtils.Model.Wrappers
{
    public class DbWrapperWorkoutProfile
    {




        #region Properties

        /// <summary>
        /// Number of WorkUnits. Randomly generated [6, 18] if not specified.
        /// </summary>
        public byte WorkUnitsNum { get; set; } = 11;

        public byte OrderNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<DbWrapperWorkUnitProfile> WorkUnits { get; set; }

        #endregion



        #region Ctors

        public DbWrapperWorkoutProfile()
        {
            WorkUnitsNum = (byte)RandomFieldGenerator.RandomInt((int)(WorkUnitsNum * 0.4f), (int)(WorkUnitsNum * 1.4f) + 1);
            WorkUnits = new List<DbWrapperWorkUnitProfile>();

            for (byte i = 0; i < WorkUnitsNum; i++)
            {
                WorkUnits.Add(new DbWrapperWorkUnitProfile()
                {
                    OrderNumber = i,
                });
            }
        }

        public DbWrapperWorkoutProfile(byte workUnitsNum)
        {
            WorkUnitsNum = workUnitsNum;
            WorkUnits = new List<DbWrapperWorkUnitProfile>();

            for (byte i = 0; i < WorkUnitsNum; i++)
            {
                WorkUnits.Add(new DbWrapperWorkUnitProfile()
                {
                    OrderNumber = i,
                });
            }
        }

        /// <summary>
        /// Builds the workout changing some parameters of the previous one.
        /// </summary>
        /// <param name="previousWorkout">Previous workout to start from</param>
        public DbWrapperWorkoutProfile(DbWrapperWorkoutProfile previousWorkout)
        {
            WorkUnits = new List<DbWrapperWorkUnitProfile>();

            foreach(DbWrapperWorkUnitProfile wu in previousWorkout.WorkUnits)
            {
                WorkUnits.Add(new DbWrapperWorkUnitProfile(wu.WorkingSets, wu.EffortType, wu.EffortValue)
                {
                    OrderNumber = wu.OrderNumber,
                });
            }

            // 
            WorkUnitsNum = (byte)WorkUnits.Count;
        }
        #endregion


        #region Public Methods

        #endregion

    }
}
