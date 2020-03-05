using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Pinturillo.Utils
{
    class InverseBoolToVisibilityConverterXBIND : IValueConverter
    {
        /// <summary>
        /// Convierte de forma inversa un booleano en un objeto de tipo Visibility
        /// 
        /// Si es true devuelve Collapse, si no, devuelve Visible
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convierte un objeto de Visibility en booleano de forma inversa.
        /// 
        /// Si es Visible devuelve false, si no, devuelve true.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}
