using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Pinturillo.Utils
{
    class TrueToFalseConverter
    : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool enabled = true;
            if ((bool)(value))  //si el valor que llega es true
            {
                enabled = false;    //se devuelve false
            }

            return enabled;
        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            return "";
        }
    }
}
