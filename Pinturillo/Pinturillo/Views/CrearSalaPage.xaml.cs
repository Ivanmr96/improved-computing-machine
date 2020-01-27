using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pinturillo
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CrearSalaPage : Page
    {
        public CrearSalaPage()
        {
            this.InitializeComponent();
            passwordbox.Visibility = Visibility.Collapsed;
        }       
            private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (checkbox.IsChecked == true) {
                passwordbox.Visibility = Visibility.Visible;
            }else
                passwordbox.Visibility = Visibility.Collapsed;
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ContentDialog confirmadoCorrectamente = new ContentDialog();
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            {
                confirmadoCorrectamente.Title = "Bienvenido";
                confirmadoCorrectamente.Content = $"Hola, {e.Parameter.ToString()}";
                confirmadoCorrectamente.PrimaryButtonText = "Aceptar";
                ContentDialogResult resultado = await confirmadoCorrectamente.ShowAsync();
            }
            else
            {
                confirmadoCorrectamente.Title = "Bienvenido";
                confirmadoCorrectamente.Content = "Hola";
                confirmadoCorrectamente.PrimaryButtonText = "Aceptar";
                ContentDialogResult resultado = await confirmadoCorrectamente.ShowAsync();
            }
            base.OnNavigatedTo(e);
        }
    }
}
