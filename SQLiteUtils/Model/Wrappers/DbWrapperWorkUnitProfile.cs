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

        public long ExcerciseId { get; set; } = 0;

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
                _effortValue = (int?)RandomFieldGenerator.FixRandomly(value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage);

                NominalReps = GetNominalRepsFromEffort();
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
                _rest = (ushort)RandomFieldGenerator.FixRandomly((float?)value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage);
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
            // Use properties
            EffortType = (GymAppSQLiteConfig.EffortType)RandomFieldGenerator.RandomInt((int)GymAppSQLiteConfig.EffortType.Intensity, (int)GymAppSQLiteConfig.EffortType.NoValue);

            EffortValue = RandomFieldGenerator.RandomEffortFromType(EffortType);

            WorkingSets = BuildWorkingSets(WorkingSetsNumber, NominalReps);
        }


        public DbWrapperWorkUnitProfile(byte nominalReps)
        {
            // Do not use properties to enforce the input parameters
            _nominalReps = nominalReps;

            _effortType = (GymAppSQLiteConfig.EffortType)RandomFieldGenerator.RandomInt((int)GymAppSQLiteConfig.EffortType.Intensity, (int)GymAppSQLiteConfig.EffortType.NoValue);

            _effortValue = (ushort?)RandomFieldGenerator.RandomEffortFromType(EffortType);

            WorkingSets = BuildWorkingSets(WorkingSetsNumber, NominalReps);
        }


        /// <summary>
        /// Builds the Work Unit slightly changing the working sets of the previous week according to a volume progression strategy
        /// </summary>
        /// <param name="workingSetsNumberPrev">Working sets number of the same workout of the previous week</param>
        /// <param name="effortTypePrev">The effort type of the previous workout sets</param>
        /// <param name="effortValPrev">The effort value of the previous workout sets</param>
        /// <param name="nominalReps">The target reps of the previous workout sets</param>
        /// <param name="makeItHarder">Make the work unit harder of the previous one (ore let it randomly change)</param>
        /// <param name="addMoreSetsProbability">Probability a new set will be added</param>
        public DbWrapperWorkUnitProfile(byte workingSetsNumberPrev, GymAppSQLiteConfig.EffortType effortTypePrev, 
            int? effortValPrev, byte nominalReps, bool makeItHarder = true, float addMoreSetsProbability = 0.2f)
        {
            // Do not use properties to enforce the input parameters
            _effortType = effortTypePrev;
            _workingSetsNumber = workingSetsNumberPrev;

            // Harder or unchanged?
            if (makeItHarder)
                (_effortValue, _nominalReps) = RandomFieldGenerator.RandomSetChange(effortTypePrev, effortValPrev, nominalReps, makeItHarder);
            else
                (_effortValue, _nominalReps) = (effortValPrev, nominalReps);

            //_nominalReps = GetNominalRepsFromEffort();

            WorkingSets = BuildWorkingSets(WorkingSetsNumber, NominalReps);

            // If the effort hasn't been changed, then add a serie
            if (makeItHarder && effortValPrev == _effortValue && nominalReps == _nominalReps)

                WorkingSets.Add(WorkingSetsNumber++,
                    (ushort)RandomFieldGenerator.FixRandomly(WorkingSets.Last().Value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage).Value);
            else
            {
                // Otherwise, randomly add some sets
                if (RandomFieldGenerator.RandomDouble(0, 1) < addMoreSetsProbability)
                    WorkingSets.Add(WorkingSetsNumber++,
                        (ushort)RandomFieldGenerator.FixRandomly(WorkingSets.Last().Value, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage).Value);
            }
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
        /// Build the working sets dictionary. Randomly fixes the values according to the class tuning parameters.
        /// </summary>
        /// <param name="workingSetsNumber">Number of working sets</param>
        /// <param name="nominalReps">Target reps</param>
        /// <param name="offsetDirection">Direction of the random offset: -1 -> decrease, +1 -> increase, 0 -> both</param>
        /// <returns></returns>
        private Dictionary<byte, ushort> BuildWorkingSets(byte workingSetsNumber, byte? nominalReps, sbyte offsetDirection = 0)
        {
            Dictionary<byte, ushort> ret = new Dictionary<byte, ushort>();

            for (byte i = 0; i < WorkingSetsNumber; i++)
                ret.Add(i, (ushort)RandomFieldGenerator.FixRandomly(
                    NominalReps, SetsParametersRandomChangeProbability, SetsParametersRandomOffsetPercentage, offsetDirection).Value);
            
            return ret;
        }

        /// <summary>
        /// Builds a valid value for the nominal reps according to the effort type and value
        /// </summary>
        /// <returns></returns>
        private byte? GetNominalRepsFromEffort()
        {
            switch (EffortType)
            {
                case GymAppSQLiteConfig.EffortType.Intensity:
                    return (byte)RandomFieldGenerator.ValidRepsFromIntensity(_effortValue.Value).Value;

                case GymAppSQLiteConfig.EffortType.RM:
                    return (byte)RandomFieldGenerator.ValidRepsFromRm(_effortValue.Value).Value;

                case GymAppSQLiteConfig.EffortType.NoValue:
                    return null;

                default:
                    return (byte)RandomFieldGenerator.RandomInt(3, 25);
            }
        }
        #endregion
    }
}
