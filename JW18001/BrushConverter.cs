using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace JW18001
{
    internal class BrushConverterIl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int i = System.Convert.ToInt32(parameter);
                var ilLowLimit = Person.IlLowerThre[i];
                var ilUpperLimit = Person.IlUpperThre[i];

                float fValue;
                if (value != null)
                {
                    fValue = System.Convert.ToSingle((string)value);
                }
                else
                {
                    return Brushes.Black;
                }
                if (fValue >= ilLowLimit && fValue < ilUpperLimit)
                {
                    return Brushes.Black;
                }
                return Brushes.Red;
            }
            catch (Exception ex)
            {
                // ignored
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class BrushConverterPdl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int i = System.Convert.ToInt32(parameter);
                var pdlLowLimit = Person.PdlLowerThre[i];
                var pdlUpperLimit = Person.PdlUpperThre[i];

                float fValue;
                if (value != null)
                {
                    fValue = System.Convert.ToSingle((string)value);
                }
                else
                {
                    return Brushes.Black;
                }
                if (fValue >= pdlLowLimit && fValue < pdlUpperLimit)
                {
                    return Brushes.Black;
                }
                return Brushes.Red;
            }
            catch (Exception ex)
            {
                // ignored
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}