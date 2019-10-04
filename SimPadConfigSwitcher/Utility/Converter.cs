using SimPadController.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace SimPadConfigSwitcher.Utility
{
    public class LightsTypeValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var s = (LightsType)value;
            return s == (LightsType)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (LightsType)int.Parse(parameter.ToString());
        }
    }

    public class ColorValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (SimPadController.Model.Color)value;

            if(color == null)
            {
                return null;
            }

            return new System.Windows.Media.Color()
            {
                R = color.R,
                G = color.G,
                B = color.B,
                A = 0xff
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (System.Windows.Media.Color?)value;

            if(!color.HasValue)
            {
                return null;
            }

            return new SimPadController.Model.Color()
            {
                R = (byte)color?.R,
                G = (byte)color?.G,
                B = (byte)color?.B
            };
        }
    }


    public class NullToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            if(parameter.ToString() == "false")
            {
                return value != null;
            }
            else
            {
                return value == null;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class NullToHiddenConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "false")
            {
                return value == null ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return value == null ? Visibility.Hidden : Visibility.Visible;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
