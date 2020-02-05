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

namespace PinturilloParaPruebas.ViewModels
{
    public class SalaEsperaVM : clsVMBase
    {
        #region atributos privados

        private clsPartida partida;
        private clsMensaje mensaje;
        private readonly INavigationService navigationService;
        private string usuarioPropio;
        private HubConnection conn;
        private IHubProxy proxy;
        #endregion

        #region constructor

        public SalaEsperaVM(INavigationService navigationService)
        {
            // Aquí obtendría la partida enviada desde la otra ventana

            partida = new clsPartida();
            this.navigationService = navigationService;
            this.salir = new DelegateCommand(salir_execute);

            SignalR();
        
            this.enviarMensaje = new DelegateCommand(enviarMensaje_execute, enviarMensaje_canExecute);
            this.mensaje = new clsMensaje();
            mensaje.JugadorQueLoEnvia = new clsJugador();
            mensaje.JugadorQueLoEnvia.Nickname = usuarioPropio;

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
            get { return this.usuarioPropio;  }
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

        private void salir_execute()
        {
            //Indica al serivdor que sale.
            proxy.Invoke("jugadorHaSalido", usuarioPropio, partida.NombreSala);

            //para probar
            //this.navigationService.NavigateTo(ViewModelLocator.ListadoSalas);
        }

        public DelegateCommand comenzarPartida { get; set; }

        public bool comenzarPartida_canExecute()
        {
            //Puede ejecutar si es el lider
            return true;
        }

        public void comenzarPartida_execute()
        {
            //Indica al servidro que la partida va a comenzar.

            navigationService.NavigateTo(ViewModelLocator.PantallaJuego);
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
            
        }

        private async void jugadorAdded(clsJugador jugador, clsPartida game)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (partida != null)
                {
                    partida.NombreSala = game.NombreSala;
                    partida.ListadoJugadores = game.ListadoJugadores;
                    partida.NotifyPropertyChanged("ListadoJugadores");
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
                }
            });
        }
        public async void  OnaddMensajeToChat (clsMensaje mensaje)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                partida.ListadoMensajes.Add(mensaje);
                partida.NotifyPropertyChanged("ListadoMensajes");

            });
        }
    }
}
