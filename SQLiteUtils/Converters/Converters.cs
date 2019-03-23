using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SQLiteUtils.Converters
{



    /// <summary>
    /// Scales down a number according to the parameter.
    /// ID: 10000, 1000 -> 10
    /// </summary>
    public class NumberScaledDownConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float scaleFactor = int.Parse(parameter.ToString());


            try
            {
                if (double.Parse(value.ToString()) == double.MaxValue)
                    return "";

                else
                {
                    return $@"{Math.Round(double.Parse(value.ToString()) / scaleFactor, 1).ToString().Replace(',','.')}";
                }
            }
            catch (Exception)
            {
                return "Error";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float scaleFactor = int.Parse(parameter.ToString());


            try
            {
                if (double.Parse(value.ToString()) == double.MaxValue)
                    return "";

                else
                    return $@"{Math.Round(double.Parse(value.ToString()) * scaleFactor, 1)}";
            }
            catch (Exception)
            {
                return "Error";
            }
        }
    }


    /// <summary>
    /// Formats an integer according a scale factor.
    /// ID: 10000, 1000 -> 10K
    /// </summary>
    public class IntToFormattedStringConverter : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            float scaleFactor = int.Parse(values[1].ToString());
            string strFactor;

            switch (scaleFactor)
            {
                case 1000:
                    strFactor = "K";
                    break;

                case 1000000:
                    strFactor = "M";
                    break;

                case 1000000000:
                    strFactor = "B";
                    break;

                default:
                    strFactor = string.Empty;
                    break;
            }

            try
            {
                if (long.Parse(values[0].ToString()) == long.MaxValue)
                    return "";

                else
                    return $@"{Math.Round(long.Parse(values[0].ToString()) / scaleFactor, 1)}{strFactor}";
            }
            catch (Exception)
            {
                return "Error";
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }




    /// <summary>
    /// Converts a pair of integers to a progress format according to the scale factor specified. 
    /// ID: (10000, 100000, 1000) -> "10K / 100K Rows"
    /// </summary>
    public class RowPairToFormattedProgressConverter : IMultiValueConverter
    {


               
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            float scaleFactor = int.Parse(values[2].ToString());
            string strFactor;

            switch(scaleFactor)
            {
                case 1000:
                    strFactor = "K";
                    break;

                case 1000000:
                    strFactor = "M";
                    break;

                case 1000000000:
                    strFactor = "B";
                    break;

                default:
                    strFactor = string.Empty;
                    break;
            }

            try
            {
                if (long.Parse(values[1].ToString()) == long.MaxValue)
                    return "";

                else
                    return $@"{Math.Round(long.Parse(values[0].ToString()) / scaleFactor , 1)}{strFactor}" +
                           $@" / {Math.Round(long.Parse(values[1].ToString()) / scaleFactor, 1)}{strFactor}  Rows";
            }
            catch(Exception)
            {
                return "Error";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
