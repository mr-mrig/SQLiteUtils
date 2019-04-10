using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace SQLiteUtils.Model.Wrappers
{
    public class DbWrapperWorkUnitProfile
    {


        #region Tuning Properties


        /// <summary>
        /// Probability that a set might differ from the nominal value (default = 10%)
        /// </summary>
        public float SetsParametersRandomChangeProbability { get; set; } = 0.1f;

        /// <summary>
        /// How much a set might differ from the nominal value (default = 10%)
        /// </summary>
        public float SetsParametersRandomOffsetPercentage { get; set; } = 0.1f;

        #endregion



        #region Properties 

        public byte OrderNumber { get; set; }

        public string Cadence { get; set; } = null;

        private byte _workingSetsNumber = 4;
        /// <summary>
        /// Number of Working Sets. (default = 4)
        /// </summary>
        public byte WorkingSetsNumber
        {
            get => _workingSetsNumber;
            set
            {
                _workingSetsNumber = value;
                WorkingSets = BuildWorkingSets(value, NominalReps);
            }
        }

        /// <summary>
        /// Nominal number of reps of the Working Sets. Each set might randomly slightly differ from this value.
        /// </summary>

        private byte? _nominalReps = 12;
        /// <summary>
        /// Number of Working Sets. (default = 4)
        /// </summary>
        public byte? NominalReps
        {
            get => _nominalReps;
            set
            {
                _nominalReps = value;
                WorkingSets = BuildWorkingSets(WorkingSetsNumber, value);
            }
        }

        private GymAppSQLiteConfig.EffortType _effortType { get; set; } = GymAppSQLiteConfig.EffortType.NoValue;

        public GymAppSQLiteConfig.EffortType EffortType
        {
            get => _effortType;
            set
            {
                _effortType = value;

                // Update the effort according to the type
                EffortValue = (ushort?)RandomFieldGenerator.RandomEffortFromType(_effortType);
            }
        }


        private int? _effortValue = null;
        /// <summary>
        /// Nominal effort of the Working Sets. Each set might randomly slightly differ from this value.
        /// </summary>
        public int? EffortValue
        {
            get => _effortValue;
            set
            {
                _effortValue = (int?)FixRandomly(value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage);

                switch(EffortType)
                {
                    case GymAppSQLiteConfig.EffortType.Intensity:
                        NominalReps = (byte)RandomFieldGenerator.ValidRepsFromIntensity(_effortValue.Value / GymAppSQLiteConfig.FloatToIntScaleFactor).Value;
                        break;

                    case GymAppSQLiteConfig.EffortType.RM:
                        NominalReps = (byte)RandomFieldGenerator.ValidRepsFromRm(_effortValue.Value).Value;
                        break;

                    case GymAppSQLiteConfig.EffortType.NoValue:
                        NominalReps = null;
                        break;

                    default:
                        NominalReps = (byte)RandomFieldGenerator.RandomInt(3, 25);
                        break;
                }
                
            }
        }


        private ushort? _rest = 90;
        /// <summary>
        /// Nominal rest of the Working Sets. Each set might randomly slightly differ from this value.
        /// </summary>
        public ushort? Rest
        {
            get => _rest;
            set
            {
                _rest = (ushort)FixRandomly((float?)value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage);
            }
        }

        /// <summary>
        /// (orderNumber, number of reps) for each Working Set
        /// </summary>
        public Dictionary<byte, ushort> WorkingSets { get; set; } = new Dictionary<byte, ushort>();
        #endregion



        #region Ctors

        public DbWrapperWorkUnitProfile()
        {
            EffortType = (GymAppSQLiteConfig.EffortType)RandomFieldGenerator.RandomInt((int)GymAppSQLiteConfig.EffortType.Intensity, (int)GymAppSQLiteConfig.EffortType.NoValue);

            EffortValue = RandomFieldGenerator.RandomEffortFromType(EffortType);

            WorkingSets = BuildWorkingSets(WorkingSetsNumber, NominalReps);
        }


        public DbWrapperWorkUnitProfile(byte nominalReps)
        {
            NominalReps = nominalReps;

            EffortType = (GymAppSQLiteConfig.EffortType)RandomFieldGenerator.RandomInt((int)GymAppSQLiteConfig.EffortType.Intensity, (int)GymAppSQLiteConfig.EffortType.NoValue);

            EffortValue = (ushort?)RandomFieldGenerator.RandomEffortFromType(EffortType);

            WorkingSets = BuildWorkingSets(WorkingSetsNumber, NominalReps);
        }


        /// <summary>
        /// Builds the Work Unit slightly changing the working sets of the previous week according to a volume progression strategy
        /// </summary>
        /// <param name="workingSetsPrev">Working sets of the same workout of the previous week</param>
        /// <param name="effortType">The effort type of the previous workout sets</param>
        /// <param name="effortVal">The effort value of the previous workout sets</param>
        /// <param name="addMoreSetsProbability">Probability a new set will be added</param>
        public DbWrapperWorkUnitProfile(Dictionary<byte, ushort> workingSetsPrev, GymAppSQLiteConfig.EffortType effortType, int? effortVal, float addMoreSetsProbability = 0.2f)
        {
            EffortType = effortType;
            EffortValue = effortVal;
            WorkingSetsNumber = (byte)workingSetsPrev.Count;

            Dictionary<byte, ushort> WorkingSets = new Dictionary<byte, ushort>();

            // Slightly modify the previous set reps
            foreach (KeyValuePair<byte, ushort> wsPrev in workingSetsPrev)
                WorkingSets.Add(wsPrev.Key, (ushort)FixRandomly(wsPrev.Value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage).Value);

            // Randomly add some sets
            if (RandomFieldGenerator.RandomDouble(0, 1) < addMoreSetsProbability)
                WorkingSets.Add(WorkingSetsNumber++, 
                    (ushort)FixRandomly(workingSetsPrev.Last().Value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage, 1).Value);

            // Randomly add some sets
            if (RandomFieldGenerator.RandomDouble(0, 1) < addMoreSetsProbability / 2f)
                WorkingSets.Add(WorkingSetsNumber++, 
                    (ushort)FixRandomly(workingSetsPrev.Last().Value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage, 1).Value);
        }
        #endregion



        #region Public Methods

        public ushort GetActualRepsFromTarget(byte setOrderNumber, float prob = 0.1f, float offset = 0.2f)
        {
            if (RandomFieldGenerator.RandomDouble(0, 1) < prob)

                return (ushort)RandomFieldGenerator.RandomInt((int)(WorkingSets[setOrderNumber] * (1 - offset)), (int)(WorkingSets[setOrderNumber] * (1 + offset)));
            else
                return WorkingSets[setOrderNumber];
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Slightly changes the value according to the porbability provided.
        /// </summary>
        /// <param name="value">The value to be fixed</param>
        /// <param name="prob">Probability of the value to be changed</param>
        /// <param name="offsetPercentage">Value percentage</param>
        /// <param name="sign">Tells the direction of the fix: -1 -> randomly decrease it, +1 -> increase it, 0 -> both directions allowed</param>
        /// <returns>The modifed value</returns>
        private float? FixRandomly(float? value, float prob, float offsetPercentage = 0.25f, sbyte sign = 0)
        {
            if (RandomFieldGenerator.RandomDouble(0, 1) < prob)
            {
                switch(sign)
                {
                    case -1:
                        return (ushort?)RandomFieldGenerator.RandomInt((int)(value * (1 - offsetPercentage)), (int)(value) + 1);

                    case +1:
                        return (ushort?)RandomFieldGenerator.RandomInt((int)(value), (int)(value * (1 + offsetPercentage)));

                    default:
                        return (ushort?)RandomFieldGenerator.RandomInt((int)(value * (1 - offsetPercentage)), (int)(value * (1 + offsetPercentage)));
                }
            }
            else
                return value;
        }


        /// <summary>
        /// Build the working sets dictionary. Randomly fixes the values according to the class tuning parameters.
        /// </summary>
        /// <param name="workingSetsNumber"></param>
        /// <param name="nominalReps"></param>
        /// <returns></returns>
        private Dictionary<byte, ushort> BuildWorkingSets(byte workingSetsNumber, byte? nominalReps)
        {
            Dictionary<byte, ushort> ret = new Dictionary<byte, ushort>();

            for (byte i = 0; i < WorkingSetsNumber; i++)
                ret.Add(i, (ushort)FixRandomly(NominalReps, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage).Value);

            return ret;
        }
        #endregion
    }
}
