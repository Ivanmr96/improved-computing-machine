using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pinturillo.Utils
{
    class BoolToVisibilityConverterXBIND : IValueConverter
    {
        /// <summary>
        /// Convierte un booleano en un objeto de tipo Visibility.
        /// 
        /// Si es true será Visible, de lo contrario será Collapsed.
        /// </summary>
        /// <param name="value">el booleano a convertir</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convierte un objeto de visibilidad en booleano.
        /// 
        /// Si es visible devuelve true, si no, false.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}
