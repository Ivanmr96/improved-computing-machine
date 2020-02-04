using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PinturilloParaPruebas.Utils
{
    class PasswordToVisibilityConverter
    : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //string visibilidad = "Visible";
            //if(String.IsNullOrEmpty( (string)value))
            //{
            //    visibilidad = "Collapsed";
            //}

            //return visibilidad;


            bool isEnabled = true;
            if (String.IsNullOrEmpty((string)value))
            {
                isEnabled = false;
            }

            return isEnabled;


        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            return "";
        }
    }
}
