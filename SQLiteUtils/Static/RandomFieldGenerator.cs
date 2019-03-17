using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteUtils
{
    public static class RandomFieldGenerator
    {

        #region consts
        public const string Alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        #endregion


        public static Random Rand = new Random();





        /// <summary>
        /// Generate a pseudo-random integer in the range specified.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <returns>A pseudo-generated random int</returns>
        public static int RandomInt(int from, int to)
        {
            return Rand.Next(from, to);
        }


        /// <summary>
        /// Generate a pseudo-random integer (incapsuled in a string) in the range specified that can be NULL.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A string storing a pseudo-generated random integer, or NULL </returns>
        public static string RandomIntNullAllowed(int from, int to, float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return "NULL";
            else
                return Rand.Next(from, to).ToString();
        }


        /// <summary>
        /// Generate a pseudo-random integer in the range specified that can be NULL.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A pseudo-generated random integer, or NULL </returns>
        public static int? RandomIntNullable(int from, int to, float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return null;
            else
                return Rand.Next(from, to);
        }


        /// <summary>
        /// Generate a pseudo-random integer selected from values in the range but the ones in the list.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="valuesExcluded"></param>
        /// <returns>A pseudo-generated random int</returns>
        public static int RandomIntValueExcluded(int from, int to, List<int> valuesExcluded)
        {
            int val = 0;

            while (valuesExcluded.Contains(val = RandomInt(from, to)))
                ;

            return val;
        }


        /// <summary>
        /// Generates a psuedo-random sequence of chars as a string.
        /// </summary>
        /// <param name="length">The legth of the string to be generated</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValue(int length)
        {
            var chars = Enumerable.Range(0, length)
                .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);

            return new string(chars.ToArray());
        }


        /// <summary>
        /// Generates a psuedo-random sequence of chars as a string. Can be NULL.
        /// </summary>
        /// <param name="length">The legth of the string to be generated</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValueNullAllowed(int length, float prob = 0.5f)
        {
            if(RandomDouble(0, 1) < prob)
                return "NULL"; ;

            var chars = Enumerable.Range(0, length)
                .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);

            return new string(chars.ToArray());
        }


        /// <summary>
        /// Generate a pseudo-random double (incapsuled in a string) in the range specifed that can be NULL.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <param name="prob">Probability of the number to be NULL</param>
        /// <returns>A string storing a pseudo-generated random double, or NULL </returns>
        public static string RandomDoubleNullAllowed(double from, double to, int decimalPlaces = 2, float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return "NULL";

            return RandomDouble(from, to, decimalPlaces).ToString();
        }


        /// <summary>
        /// Generate a pseudo-random double (incapsuled in a string) in the range specifed.
        /// </summary>
        /// <param name="from">Range lower bound</param>
        /// <param name="to">Range upper bound</param>
        /// <returns>A string storing a pseudo-generated random double </returns>
        public static double RandomDouble(double from, double to, int decimalPlaces = 2)
        {
            return Math.Round(Rand.NextDouble() * (to - from) + from, decimalPlaces);
        }



        /// <summary>
        /// Returns true with the provided probability
        /// </summary>
        /// <param name="prob">Probability of True</param>
        /// <returns>Returns a boolean</returns>
        public static bool RandomBoolWithProbability(float prob = 0.5f)
        {
            if (Rand.NextDouble() < prob)
                return true;
            else
                return false;
        }


        /// <summary>
        /// Generates a Unix-formatted pseudo-random DateTime inside the specific range. Can be null with a specific probability.
        /// </summary>
        /// <param name="start">Range lower bound</param>
        /// <param name="end">Range higher bound</param>
        /// <param name="prob">Probability of returning a null value</param>
        /// <returns></returns>
        public static int? RandomUnixTimestamp(DateTime start, DateTime end, float prob = 0f)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            // Check Dates are subsequent the starting Unix date
            if ((DatabaseUtility.UnixTimestampT0 - start).TotalMilliseconds > 0 || (DatabaseUtility.UnixTimestampT0 - end).TotalMilliseconds > 0)
                return null;

            // Check end date is subsequent to start date
            if ((start - end).TotalMilliseconds > 0)
                return null;


            int secondsBetween = (int)(end - start).TotalSeconds;

            return Rand.Next(secondsBetween) + (int)(start - DatabaseUtility.UnixTimestampT0).TotalSeconds;
        }


        public static string RandomDateTimeNullAllowed(DateTime start, DateTime end, float nullProb = 0.2f)
        {
            if (Rand.NextDouble() <= nullProb)
                return "'NULL'";

            return RandomUnixTimestamp(start, end).ToString();
        }


        /// <summary>
        /// Generates a Unix-formatted pseudo-random Date (no time) inside the specific range.
        /// </summary>
        /// <param name="start">Range lower bound</param>
        /// <param name="end">Range higher bound</param>
        /// <param name="prob">Probability of returning a null value</param>
        /// <returns>The pseudo-random integer representing a Unix formatted Date, or null</returns>
        public static int? RandomUnixDate(DateTime start, DateTime end, float prob = 0f)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            // Check Dates are subsequent the starting Unix date
            if ((DatabaseUtility.UnixTimestampT0 - start).TotalMilliseconds > 0 || (DatabaseUtility.UnixTimestampT0 - end).TotalMilliseconds > 0)
                return null;

            // Check end date is subsequent to start date
            if ((start - end).TotalMilliseconds > 0)
                return null;

            // Truncate to Date
            DateTime start0 = start.Date;
            DateTime end0 = end.Date;

            // Difference in terms of days
            int daysBetween = (int)(end0 - start0).TotalDays;

            // Random truncated to date
            DateTime randomDate = start0.AddDays(Rand.Next(daysBetween));


            return  (int)(randomDate - DatabaseUtility.UnixTimestampT0).TotalSeconds;
        }


        /// <summary>
        /// Generates a Unix-formatted pseudo-random Date (no time) inside the specific range. Can return null.
        /// </summary>
        /// <param name="start">Range lower bound as Unix timestamp</param>
        /// <param name="minIncrease">Minimum increase (as Unix timestamp) to be added to the start parameter </param>
        /// <param name="maxIncrease">Maximum increase (as Unix timestamp) to be added to the start parameter </param>
        /// <param name="prob">Probability of returning a null value</param>
        /// <returns>The pseudo-random integer representing a Unix formatted Date, or null</returns>
        public static int? RandomUnixDate(int start, int minIncrease, int maxIncrease, float prob = 0f)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            // Check Dates are subsequent the starting Unix date
            if (minIncrease >= maxIncrease)
                return start;

            return Rand.Next(start + minIncrease, start + maxIncrease);
        }


        /// <summary>
        /// Generate a default Random field according to the Affinity Type
        /// </summary>
        /// <param name="colType">The Affinity type for the colum</param>
        /// <returns>A default random value</returns>
        public static string GenerateRandomField(TypeAffinity colType)
        {
            string colValue = string.Empty;
            Random rand = new Random();

            switch (colType)
            {
                case TypeAffinity.Text:


                    colValue = $"'{((char)(rand.Next(1, 26) + 64)).ToString() }'";
                    break;

                case TypeAffinity.Int64:

                    colValue = $"'{new Random().Next(0, Int32.MaxValue)}'";
                    break;

            }

            return colValue;
        }


    }
}
