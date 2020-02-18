using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PinturilloParaPruebas.Utils
{
    class LapizVisibilityConverter
    : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string visibilidad = "Visible";
            if (!(bool)(value))
            {
                visibilidad = "Collapsed";
            }

            return visibilidad;
        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            return "";
        }
    }
}
