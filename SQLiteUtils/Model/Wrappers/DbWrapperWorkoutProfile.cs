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
        #endregion


        #region Public Methods

        #endregion

    }
}
