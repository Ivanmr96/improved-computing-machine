using Pinturillo;
using Pinturillo.Models;
using Pinturillo.ViewModels;
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
    public sealed partial class ListadoSalas : Page
    {

        private VMListadoSalas viewModel { get; }

        public ListadoSalas()
        {
            this.InitializeComponent();
            viewModel = (VMListadoSalas)this.DataContext;

            //List<User> items = new List<User>();
            //items.Add(new User() { Name = "Sala Los Mejores", Age = 42 });
            //items.Add(new User() { Name = "Depression Developers", Age = 39 });
            //items.Add(new User() { Name = "Sala 0", Age = 13 });
            //listadoSalas.ItemsSource = items;
        }


        /// <summary>
        /// Evento asociado al click del botón de volver atrás.
        /// 
        /// Navega hacia la primera pantalla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackArrow_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }


        /// <summary>
        /// Se ejecuta cuando se ha navegado a esta pantalla.
        /// 
        /// Guarda el nombre de usuario recibido desde la anterior pantalla.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                viewModel.UsuarioPropio.Nickname = e.Parameter.ToString();
            }
            //Frame.BackStack.Clear();
            base.OnNavigatedTo(e);
            
        }

        /// <summary>
        /// Evento ascoiado al click en algún elemento de la lista de salas.
        /// 
        /// Se establece como seleccionada la sala que se haya pulsado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListadoSalas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            clsPartida partida = (clsPartida)((FrameworkElement)e.OriginalSource).DataContext;

            viewModel.ListadoSalas_Tapped(partida);
        }
    }
}
