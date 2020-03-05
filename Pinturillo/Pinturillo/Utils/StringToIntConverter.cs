using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Pinturillo.Utils
{
    class StringToIntConverter : IValueConverter
    {
        /// <summary>
        /// Convierte un int en string.
        /// </summary>
        /// <param name="value">el int a convertir en string</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        /// <summary>
        /// Convierte un string en int
        /// </summary>
        /// <param name="value">el string a convertir en int</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            
            return int.Parse((String)value);
        }
    }
}
