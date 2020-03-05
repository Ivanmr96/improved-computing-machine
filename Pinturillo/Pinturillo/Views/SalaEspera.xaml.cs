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

        /// <summary>
        /// Se ejecuta cuando se haya navegado a esta página.
        /// 
        /// Si la anterior página fue la de crear la sala.
        /// 
        /// Obtiene la partida recibida de la anterior pantalla, establece al jugador como el lider (ya que es el creador)
        /// 
        /// Si no, significa que viene de la pantalla del listado de salas, por lo que obtiene tanto el nombre de usuario como el nombre de la sala.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;
            //Frame.BackStack.Clear();
            //GC.Collect();
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
                    //TODO aqui hace falta pillar el nombre de la partida que has tappeado

                    //Tupla item 1 nick, item 2 nombre partida

                    viewModel.UsuarioPropio = ((Tuple<string,string>)e.Parameter).Item1;
                    viewModel.Partida.NombreSala = ((Tuple<string, string>)e.Parameter).Item2;
                }
            }
            base.OnNavigatedTo(e);

        }

        /// <summary>
        /// Evento asociado a la pulsación de la tecla Enter.
        /// 
        /// Si el comando enviarMensaje se puede ejecutar, se manda el mensaje escrito en el campo de texto para el chat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxMensaje_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (viewModel.enviarMensaje_canExecute())
                {
                    viewModel.enviarMensaje_execute();
                }

            }
        }
    }
}
