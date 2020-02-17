using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using PinturilloParaPruebas;
using PinturilloParaPruebas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PinturilloParaPruebas.ViewModels
{
    public class SalaEsperaVM : clsVMBase
    {
        #region atributos privados

        private clsPartida partida;
        private clsMensaje mensaje;
        Frame navigationFrame = Window.Current.Content as Frame;
        private string usuarioPropio;
        private HubConnection conn;
        private IHubProxy proxy;
        #endregion

        #region constructor

        public SalaEsperaVM()
        {
            // Aquí obtendría la partida enviada desde la otra ventana

            partida = new clsPartida();
            this.salir = new DelegateCommand(salir_execute);

            SignalR();

            this.enviarMensaje = new DelegateCommand(enviarMensaje_execute, enviarMensaje_canExecute);
            this.comenzarPartida = new DelegateCommand(comenzarPartida_execute, comenzarPartida_canExecute);
            this.mensaje = new clsMensaje();
            mensaje.JugadorQueLoEnvia = new clsJugador();

            //partida.ListadoJugadores.Add(new clsJugador("id", 0, "Ivan", false, false, false));
            //partida.ListadoJugadores.Add(new clsJugador("id", 0, "Pepe", false, false, false));
        }

        #endregion

        #region propiedades publicas

        public clsPartida Partida
        {
            get => partida;
            set
            {
                partida = value;
                partida.NotifyPropertyChanged("ListadoJugadores");

            }
        }
        public clsMensaje Mensaje
        {
            get => mensaje;
            set => mensaje = value;
        }

        public string UsuarioPropio
        {
            get { return this.usuarioPropio; }
            set { this.usuarioPropio = value; }
        }

        #endregion

        #region commands

        public DelegateCommand enviarMensaje { get; set; }

        private bool enviarMensaje_canExecute() => mensaje != null || mensaje.Mensaje != "";

        private void enviarMensaje_execute()
        {
            //Mandar el mensaje al servidor

            proxy.Invoke("sendMensaje", mensaje, partida.NombreSala);
        }

        public DelegateCommand salir { get; set; }

        private async void salir_execute()
        {

            ContentDialog confirmadoCorrectamente = new ContentDialog();
            confirmadoCorrectamente.Title = "Confirmación";
            confirmadoCorrectamente.Content = "¿Seguro que quieres salir?";
            confirmadoCorrectamente.PrimaryButtonText = "Si";
            confirmadoCorrectamente.SecondaryButtonText = "No";
            ContentDialogResult resultado = await confirmadoCorrectamente.ShowAsync();
            if (resultado == ContentDialogResult.Primary)
            {
                //this.Frame.Navigate(typeof(ListadoSalas));
                navigationFrame.Navigate(typeof(ListadoSalas), usuarioPropio);
                await proxy.Invoke("jugadorHaSalido", usuarioPropio, partida.NombreSala);
            }
        }

        public DelegateCommand comenzarPartida { get; set; }

        public bool comenzarPartida_canExecute()
        {
            bool puedeComenzar = false;
            clsJugador jugador;
            try
            {
                jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuarioPropio);
            }
            catch (Exception e)
            {
                jugador = null;
            }
            if (jugador != null)
            {
                if (jugador.IsLider && partida.ListadoJugadores.Count >= 2)
                {
                    puedeComenzar = true;
                }
            }
            //Puede ejecutar si es el lider
            return puedeComenzar;
        }

        public void comenzarPartida_execute()
        {
            //Indica al servidor que la partida va a comenzar.

            partida.IsJugandose = true;
            proxy.Invoke("empezarPartida", partida.NombreSala);
            Tuple<String, clsPartida> partidaConNick = new Tuple<string, clsPartida>(usuarioPropio, partida);
            navigationFrame.Navigate(typeof(PantallaJuego), partidaConNick);
        }

        #endregion


        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;
            //conn = new HubConnection("http://localhost:11111/");
            //proxy = conn.CreateHubProxy("PictionaryHub");
            //await conn.Start();

            proxy.On<clsMensaje>("addMensajeToChat", OnaddMensajeToChat);
            proxy.On<clsJugador, clsPartida>("jugadorAdded", jugadorAdded);
            proxy.On<string, string>("jugadorDeletedSala", OnjugadorDeleted);
            proxy.On("empezarPartida", OnempezarPartidaAsync);
            proxy.On("nombrarComoLider", OnnombrarComoLider);
        }

        private async void OnempezarPartidaAsync()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                partida.IsJugandose = true;
                Tuple<String, clsPartida> partidaConNick = new Tuple<string, clsPartida>(usuarioPropio, partida);
                navigationFrame.Navigate(typeof(PantallaJuego), partidaConNick);
               
            });
        }
        //se nombra como lider al jugador actual
        private async void OnnombrarComoLider()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clsJugador jugador;
                try
                {
                    jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuarioPropio);
                }
                catch (Exception e)
                {
                    jugador = null;
                }

                if (jugador != null)
                {
                    jugador.IsLider = true;
                    comenzarPartida.RaiseCanExecuteChanged();
                }
            });
        }

        private async void jugadorAdded(clsJugador jugador, clsPartida game)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (partida != null)
                {
                    mensaje.JugadorQueLoEnvia.Nickname = usuarioPropio;
                    NotifyPropertyChanged("Mensaje");
                    /*partida.NombreSala = game.NombreSala;
                    partida.ListadoMensajes = game.ListadoMensajes;
                    partida.NotifyPropertyChanged("ListadoMensajes");
                    partida.ListadoJugadores = game.ListadoJugadores;
                    partida.NotifyPropertyChanged("ListadoJugadores");
                    comenzarPartida.RaiseCanExecuteChanged();*/
                    partida = game;
                    NotifyPropertyChanged("Partida");
                    partida.NotifyPropertyChanged("ListadoJugadores");
                    partida.NotifyPropertyChanged("ListadoMensajes");
                    comenzarPartida.RaiseCanExecuteChanged();
                    //NotifyPropertyChanged("partidasAMostrar");

                }
            });
        }

        public async void OnjugadorDeleted(string usuario, string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clsJugador jugador;
                try
                {
                    jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuario);
                }
                catch (Exception e)
                {
                    jugador = null;
                }

                if (jugador != null)
                {
                    partida.ListadoJugadores.Remove(jugador);
                    partida.NotifyPropertyChanged("ListadoJugadores");
                    comenzarPartida.RaiseCanExecuteChanged();
                }
            });
        }

        public async void OnaddMensajeToChat(clsMensaje mensaje)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                partida.ListadoMensajes.Add(mensaje);
                partida.NotifyPropertyChanged("ListadoMensajes");

            });
        }

    }
}
