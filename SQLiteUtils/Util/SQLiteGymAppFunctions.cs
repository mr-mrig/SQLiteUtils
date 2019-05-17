using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SQLiteUtils.Util
{

    public static class SQLiteGymAppFunctions
    {




        [SQLiteFunction(Name = "RmToIntensityPerc", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class RmToIntensityPercSQLiteFunction : SQLiteFunction
        {

            public override object Invoke(object[] args)
            {
                return 0.4167 * Convert.ToInt32(args[0]) - 14.2831 * Math.Pow(Convert.ToInt32(args[0]), 0.5) + 115.6122;
            }
        }



        [SQLiteFunction(Name = "IntensityPercToRm", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class IntensityPercToRmSQLiteFunction : SQLiteFunction
        {

            public override object Invoke(object[] args)
            {
                return 324.206809067032 - 18.0137586362208 * Convert.ToDouble(args[0]) + 0.722425494099458 * Math.Pow(Convert.ToDouble(args[0]), 2) - 0.018674659779516 * Math.Pow(Convert.ToDouble(args[0]), 3) + 0.00025787003728422 * Math.Pow(Convert.ToDouble(args[0]), 04)
                    - 1.65095582844966E-06 * Math.Pow(Convert.ToDouble(args[0]), 5) + 2.75225269851 * Math.Pow(10, -9) * Math.Pow(Convert.ToDouble(args[0]), 6) + 8.99097867 * Math.Pow(10, -12) * Math.Pow(Convert.ToDouble(args[0]), 7);
            }
        }




        [SQLiteFunction(Name = "EffortToRpe", Arguments = 3, FuncType = FunctionType.Scalar)]
        public class EffortToRpeSQLiteFunction : SQLiteFunction
        {

            // TCL
            //set effort[lindex $argv 0]
            //set effortType[lindex $argv 1]
            //set targetReps[lindex $argv 2]

            //if {$effortType == 1} {
            //      set intensity[expr { $effort / 10.0 }]
            //      set rm[expr { 324.206809067032 - 18.0137586362208 * $intensity + 0.722425494099458 * pow($intensity, 2) - 0.018674659779516 * pow($intensity, 3) + 0.00025787003728422 * pow($intensity, 04) - 1.65095582844966E-06 * pow($intensity, 5) + 2.75225269851 * pow(10, -9) * pow($intensity, 6) + 8.99097867 * pow(10, -12) * pow($intensity, 7) }]
            //      set val[expr { 10 - ($rm - $targetReps) }]
            //      set retval[expr round($val)]

            //} elseif {$effortType == 2} {
            //   set retval[expr { 10 - ($effort - $targetReps) }]
            //} elseif {$effortType == 3} {
            //   return $effort
            //} else {
            //   return 0
            //}	

            //if {$retval > 4} {
            //   return $retval 
            //} else {
            //   return 4
            //}

            public override object Invoke(object[] args)
            {
                try
                {
                    int effort = Convert.ToInt32(args[0]);
                    int effortType = Convert.ToInt32(args[1]);
                    int targetReps = Convert.ToInt32(args[2]);


                    switch (effortType)
                    {

                        case 1:

                            // Intensity
                            double rm = 0.4167 * effort / 10.0f - 14.2831 * Math.Pow(effort / 10.0f, 0.5) + 115.6122;
                            return Math.Max(Math.Round(10 - (rm - targetReps), 0), 4);

                        case 2:

                            // Rm
                            return Math.Max(10 - (effort - targetReps), 4);

                        case 3:

                            // RPE
                            return effort;

                        default:
                            return 0;

                    }
                }
                catch
                {
                    return null;
                }
            }
        }





        [SQLiteFunction(Name = "EffortToIntensityPerc", Arguments = 3, FuncType = FunctionType.Scalar)]
        public class EffortToIntensityPercSQLiteFunction : SQLiteFunction
        {

            // TCL
            //set effort[lindex $argv 0]
            //set effortType[lindex $argv 1]
            //set targetReps[lindex $argv 2]

            //if {$effortType == 1} {

            //   expr { $effort / 10.0 }

            //} elseif {$effortType == 2} {

            //      set intensity[expr { 0.4167 * $effort - 14.2831 * pow($effort, 0.5) + 115.6122 }]
            //      expr round($intensity* 10.0) / 10.0

            //} elseif {$effortType == 3} {

            //      set param[expr { $targetReps + (10 - $effort) }]
            //      set intensity[expr { 0.4167 * $param - 14.2831 * pow($param, 0.5) + 115.6122 }]
            //      expr round($intensity)

            //} else {
            //   return 0
            //}	

            public override object Invoke(object[] args)
            {
                try
                {
                    int effort = Convert.ToInt32(args[0]);
                    int effortType = Convert.ToInt32(args[1]);
                    int targetReps = Convert.ToInt32(args[2]);

                    switch (effortType)
                    {

                        case 1:

                            // Intensity
                            return effort / 10.0;

                        case 2:

                            // Rm
                            return Math.Round(0.4167 * effort - 14.2831 * Math.Pow(effort, 0.5) + 115.6122, 1);

                        case 3:

                            // RPE
                            int rm = targetReps + (10 - effort);
                            return Math.Round(0.4167 * rm - 14.2831 * Math.Pow(rm, 0.5) + 115.6122, 1);

                        default:
                            return 0;

                    }
                }
                catch
                {
                    return null;
                }
            }
        }





    }
}
