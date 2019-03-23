using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SQLiteUtils.Converters
{


    public class TwoIntValuesToStringConverter : IMultiValueConverter
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
