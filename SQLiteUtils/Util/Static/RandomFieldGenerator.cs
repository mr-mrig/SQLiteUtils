﻿using SQLiteUtils.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            try
            {
                return Rand.Next(from, to);
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debugger.Break();
                throw exc;
            }
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
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValue(int length, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            var chars = Enumerable.Range(0, length)
                .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);

            return new string(chars.ToArray());
        }



        /// <summary>
        /// Generates a psuedo-random sequence of chars as a string.
        /// </summary>
        /// <param name="lengthMin">The minimum legth of the string to be generated</param>
        /// <param name="lengthMax">The maximum legth of the string to be generated</param>
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>A pseudo-random string.</returns>
        public static string RandomTextValue(int lengthMin, int lengthMax, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            var chars = Enumerable.Range(0, RandomInt(lengthMin, lengthMax))
                .Select(x => Alphabet[Rand.Next(0, Alphabet.Length)]);

            return new string(chars.ToArray());
        }




        /// <summary>
        /// Randomly choose an element among the choices provided.
        /// </summary>
        /// <param name="possibleChoices">The items list which to choose from</param>
        /// <returns>One of the possible items.</returns>
        public static T? ChooseAmong<T>(List<T?> possibleChoices, float prob = 0) where T : struct
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            return possibleChoices[RandomInt(0, possibleChoices.Count)];
        }



        /// <summary>
        /// Randomly choose a string among the choices provided.
        /// </summary>
        /// <param name="possibleChoices">The list of string which to choose from</param>
        /// <param name="prob">Probability of null being returned</param>
        /// <returns>One of the specified strings.</returns>
        public static string ChooseText(List<string> possibleChoices, float prob = 0)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            return possibleChoices[RandomInt(0, possibleChoices.Count)];
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


        /// <summary>
        /// Generates a Unix-formatted pseudo-random DateTime inside the specific range. Can be null with a specific probability.
        /// </summary>
        /// <param name="start">Range lower bound as Unix timestamp</param>
        /// <param name="minIncrease">Minimum increase (as Unix timestamp) to be added to the start parameter </param>
        /// <param name="maxIncrease">Maximum increase (as Unix timestamp) to be added to the start parameter </param>
        /// <param name="prob">Probability of returning a null value</param>
        /// <returns></returns>
        public static int? RandomUnixTimestamp(int start, int minIncrease, int maxIncrease, float prob = 0f)
        {
            if (RandomDouble(0, 1) < prob)
                return null;

            // Check Dates are subsequent the starting Unix date
            if (minIncrease >= maxIncrease)
                return start;

            return Rand.Next(start + minIncrease, start + maxIncrease);
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


            return (int)(randomDate - DatabaseUtility.UnixTimestampT0).TotalSeconds;
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
        /// Randomly choose the number of reps according to the selected intensity [%].
        /// </summary>
        /// <param name="intensityPercentage">The intensity in terms of RM percentage</param>
        /// <returns>A random number which respects the intensity boundaries.</returns>
        public static int? ValidRepsFromIntensity(float intensityPercentage, float prob = 0)
        {
            float step = 5f;

            if (intensityPercentage > 94)
                return 1;
            else if (intensityPercentage >= 83 && intensityPercentage <= 94)
                step = 3;
            else if (intensityPercentage >= 68)
                step = 2.5f;

            else if (intensityPercentage >= 63)
                step = 2.2f;

            else
                step = 2f;

            // Maximum valid reps to choose from
            int maxReps = (int)Math.Round((100f - intensityPercentage) / step);

            return RandomIntNullable((int)(maxReps * 0.5f), maxReps + 1, prob);
        }


        /// <summary>
        /// Randomly choose the number of reps according to the selected intensity [RM].
        /// </summary>
        /// <param name="intensityPercentage">The intensity in terms of RM weight</param>
        /// <returns>A random number which respects the intensity boundaries.</returns>
        public static int? ValidRepsFromRm(int intensityRM, float prob = 0)
        {
            return RandomIntNullable((int)(intensityRM * 0.7f), intensityRM + 1, prob);
        }


        /// <summary>
        /// Randomly choose the number of reps according to the selected intensity [RM].
        /// </summary>
        /// <param name="intensityPercentage">The intensity in terms of RM weight</param>
        /// <returns>A random number which respects the intensity boundaries.</returns>
        public static int? RandomEffortFromType(GymAppSQLiteConfig.EffortType effortType, float prob = 0)
        {
            int? effort;
            
            if (RandomDouble(0, 1) < prob)
                return null;

            switch (effortType)
            {
                case GymAppSQLiteConfig.EffortType.Intensity:

                    effort = (int)(RandomDouble(50, 90, 1) * GymAppSQLiteConfig.FloatToIntScaleFactor);
                    break;

                case GymAppSQLiteConfig.EffortType.RM:

                    effort = RandomInt(3, 20);
                    break;


                case GymAppSQLiteConfig.EffortType.RPE:

                    effort = RandomInt(6, 10);
                    break;

                default:

                    effort = null;
                    break;

            }

            return effort;
        }


        /// <summary>
        /// Apply changes to a working set
        /// </summary>
        /// <param name="effortType">Effort type of the set to be modified</param>
        /// <param name="effortValue">Effort value of the set to be modified</param>
        /// <param name="targetReps">Target reps of the set to be modified</param>
        /// <param name="makeItHarder">Make it more difficult or apply randomic changes</param>
        /// <returns>The new working as a tuple (intensity, target repetitions)</returns>
        public static (int?, byte) RandomSetChange(GymAppSQLiteConfig.EffortType effortType, int? effortValue, byte targetReps, bool makeItHarder = false)
        {
            bool intensityProgression;
            int? effort;
            byte nominalReps;

            switch (effortType)
            {
                case GymAppSQLiteConfig.EffortType.Intensity:

                    if(makeItHarder)
                    {
                        // Chose between Intensity or Volume
                        intensityProgression = RandomDouble(0, 1) < 0.5f;

                        if (intensityProgression)
                        {
                            // Add Intensity
                            if (effortValue < 700)
                                effort = (int?)(1.05f * effortValue);
                            else if (effortValue < 930)
                                effort = (int?)(1.025f * effortValue);
                            else
                            {
                                // Intensity progression not possible: add one rep
                                effort = effortValue;
                                nominalReps = (byte)(targetReps + 1);
                                break;
                            }

                            nominalReps = (byte)ValidRepsFromIntensity(effort.Value).Value;
                        }
                        else
                        {
                            // Add one rep
                            effort = effortValue;
                            nominalReps = (byte)(targetReps + 1);
                        }
                    }
                    else
                    {
                        //// Random changes
                        //effort = (ushort)FixRandomly(effortValue, 0.2f);
                        //nominalReps = (byte)ValidRepsFromIntensity(effort.Value).Value;
                        effort = effortValue;
                        nominalReps = targetReps;
                    }
                    break;

                case GymAppSQLiteConfig.EffortType.RM:

                    if (makeItHarder)
                    {
                        // Chose between Intensity or Volume
                        intensityProgression = RandomDouble(0, 1) < 0.5f;

                        if (intensityProgression)
                        {
                            // Add Intensity
                            if (effortValue > 15)
                                effort = effortValue - 2;
                            else if (effortValue > 3)
                                effort = effortValue - 1;
                            else
                            {
                                // Intensity progression not possible: add one rep
                                nominalReps = (byte)(targetReps + 1);
                                // Sometimes decrease the effort (IE: increase the RM)
                                if(RandomDouble(0, 1) < 0.5f)
                                    effort = effortValue + 1;
                                else
                                    effort = effortValue;

                                break;
                            }

                            nominalReps = (byte)ValidRepsFromRm(effort.Value).Value;
                        }
                        else
                        {
                            // Add one rep
                            effort = effortValue;
                            nominalReps = (byte)(targetReps + 1);
                        }
                    }
                    else
                    {
                        //// Random changes
                        //effort = (ushort)FixRandomly(effortValue, 0.2f);
                        //nominalReps = (byte)ValidRepsFromRm(effort.Value).Value;
                        effort = effortValue;
                        nominalReps = targetReps;
                    }
                    break;


                case GymAppSQLiteConfig.EffortType.RPE:

                    if(makeItHarder)
                    {
                        if (effortValue < 8)
                        {
                            // Increase intensity
                            effort = effortValue + 1;
                            nominalReps = targetReps;
                        }
                        else
                        {
                            // Increase volume
                            effort = effortValue;
                            nominalReps = (byte)(targetReps + 1);
                        }
                    }
                    else
                    {
                        //// Random changes
                        //effort = (ushort)FixRandomly(effortValue, 0.05f);
                        //nominalReps = (byte)FixRandomly(targetReps, 0.05f);
                        effort = effortValue;
                        nominalReps = targetReps;
                    }

                    break;

                default:

                    effort = -1;
                    nominalReps = 0;
                    break;

            }

            return (effort, nominalReps);
        }


        /// <summary>
        /// Slightly changes the value according to the porbability provided.
        /// </summary>
        /// <param name="value">The value to be fixed</param>
        /// <param name="prob">Probability of the value to be changed</param>
        /// <param name="offsetPercentage">Value percentage</param>
        /// <param name="sign">Tells the direction of the fix: -1 -> randomly decrease it, +1 -> increase it, 0 -> both directions allowed</param>
        /// <returns>The modifed value</returns>
        public static float? FixRandomly(float? value, float prob, float offsetPercentage = 0.25f, sbyte sign = 0)
        {
            if (RandomDouble(0, 1) < prob)
            {
                switch (sign)
                {
                    case -1:
                        return (ushort?)RandomInt((int)(value * (1 - offsetPercentage)), (int)(value) + 1);

                    case +1:
                        return (ushort?)RandomInt((int)(value), (int)(value * (1 + offsetPercentage)));

                    default:
                        return (ushort?)RandomInt((int)(value * (1 - offsetPercentage)), (int)(value * (1 + offsetPercentage)));
                }
            }
            else
                return value;
        }


        /// <summary>
        /// Generate a default Random field according to the Affinity Type
        /// </summary>
        /// <param name="colType">The Affinity type for the colum</param>
        /// <returns>A default random value</returns>
        public static DatabaseColumnWrapper GenerateRandomField(DatabaseColumnWrapper column)
        {

            // Reset the value
            column.Value = null;

            // Check if column is among specific well-known fields
            switch (column.Name)
            {

                case "Body":
                case "Caption":
                case "Message":
                case "Msg":
                case "Description":

                    column.Value = RandomTextValue(10, 200);
                    break;

                case "Name":
                case "Key":
                case "Title":

                    column.Value = RandomTextValue(5, 20);
                    break;

                case "Abbreviation":

                    column.Value = RandomTextValue(2, 5);
                    break;

                case "CreatedOn":
                case "LastUpdate":

                    column.Value = RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);
                    break;

            }

            if (column.Value == null
                && (column.Name.Contains("Number") || column.Name.Contains("Grams") || column.Name.Contains("Kg") || column.Name.Contains("Liters")))

                column.Value = RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);

            // DateTimes
            if (column.Value == null
                && (column.Name.Contains("Date") || column.Name.Contains("Timestamp") || column.Name.Contains("Time")))

                column.Value = RandomUnixTimestamp(GymAppSQLiteConfig.DbDateLowerBound, GymAppSQLiteConfig.DbDateUpperBound);

            // Boolean
            if (column.Value == null && Regex.IsMatch(column.Name, "Is[a-zA-Z_]+"))

                column.Value = RandomInt(0, 2);

            // Rating
            if (column.Value == null && column.Name.Contains("Rating"))

                column.Value = RandomInt(0, 5);

            // FK
            if (column.Value == null && Regex.IsMatch(column.Name, "[a-zA-Z_]Id"))

                column.Value = RandomInt(0, 4);


            // Otherwise make it random according to its type
            if (column.Value == null)
            {
                switch (column.Affinity)
                {
                    case TypeAffinity.Text:


                        column.Value = RandomTextValue(10, 20);
                        break;

                    case TypeAffinity.Int64:

                        column.Value = RandomInt(0, 100);
                        break;


                    case TypeAffinity.Double:

                        column.Value = RandomDouble(0, 100);
                        break;

                    default:
                        column.Value = RandomTextValue(5, 15);
                        break;

                }
            }
            return column;
        }


    }
}
