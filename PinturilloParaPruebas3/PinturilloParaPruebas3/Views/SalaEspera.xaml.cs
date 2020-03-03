using PinturilloParaPruebas3.Models;
using PinturilloParaPruebas3.ViewModels;
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

namespace PinturilloParaPruebas3
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;
            //Frame.BackStack.Clear();
            //GC.Collect();
            //if (e.SourcePageType.FullName.Equals("PinturilloParaPruebas3.CrearSalaPage"))
            if (lastPage.FullName.Equals("PinturilloParaPruebas3.CrearSalaPage"))
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
    }
}
