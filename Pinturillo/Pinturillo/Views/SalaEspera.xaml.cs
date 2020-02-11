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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace Pinturillo
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SalaEspera : Page
    {


        private SalaEsperaVM viewModel { get; }
        public SalaEspera()
        {
            this.InitializeComponent();
            viewModel = (SalaEsperaVM)DataContext;
            btnEnviarMensaje.Focus(FocusState.Keyboard);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;

            //if (e.SourcePageType.FullName.Equals("Pinturillo.CrearSalaPage"))
            if (lastPage.FullName.Equals("Pinturillo.CrearSalaPage"))
            {
                if (e.Parameter != null)
                {
                    viewModel.Partida = (clsPartida)e.Parameter;

                    clsJugador jugadorLider = viewModel.Partida.ListadoJugadores.First<clsJugador>(x => x.IsLider);

                    //viewModel.Partida.ListadoJugadores.

                    viewModel.UsuarioPropio = jugadorLider.Nickname;
                    viewModel.Mensaje.JugadorQueLoEnvia = jugadorLider;
                }
            }else
            {
                if(e.Parameter != null)
                {
                    viewModel.UsuarioPropio = (string)e.Parameter;
                   
                }
            }
            base.OnNavigatedTo(e);

        }
    }
}
