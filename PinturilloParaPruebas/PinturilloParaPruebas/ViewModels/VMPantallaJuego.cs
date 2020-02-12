using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using PinturilloParaPruebas.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PinturilloParaPruebas.ViewModels
{
    public class VMPantallaJuego : clsVMBase
    {
        #region"Atributos privados"
        private ObservableCollection<clsMensaje> _listadoMensajesChat;
        private clsJugador _usuarioPropio;
        private clsMensaje _mensaje;
        private int _timeMax = 60;
        private DispatcherTimer _dispatcherTimer;
        private clsPartida _partida;
        private string _lblTemporizador;
        private HubConnection conn;
        private IHubProxy proxy;
        private readonly INavigationService navigationService;
        #endregion

        public VMPantallaJuego(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            _partida = new clsPartida();
            this._mensaje = new clsMensaje();
            _usuarioPropio = new clsJugador();
            _mensaje.JugadorQueLoEnvia = new clsJugador();
            this.GoBackCommand = new DelegateCommand(ExecuteGoBackCommand);
            this.SendMessageCommand = new DelegateCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand);

            this._dispatcherTimer = new DispatcherTimer();
            this._dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            this._dispatcherTimer.Tick += Timer_Tik;
            this._dispatcherTimer.Start();
            this.LblTemporizador = "60";

            SignalR();
        }


        private void Timer_Tik(object sender, object e)
        {
            if (_timeMax > 0)
            {

                if (_timeMax <= 10)
                {
                    _timeMax--;
                    LblTemporizador = string.Format("0{1}", _timeMax / 60, _timeMax % 60);
                    NotifyPropertyChanged("LblTemporizador");
                }
                else
                {
                    _timeMax--;
                    LblTemporizador = string.Format("{1}", _timeMax / 60, _timeMax % 60);
                    NotifyPropertyChanged("LblTemporizador");
                }
            }
            else
            {
                this._dispatcherTimer.Stop();


                if (_timeMax == 0)
                {
                    //TODO 
                }
            }
        }

        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;
            //conn = new HubConnection("http://localhost:11111/");
            //proxy = conn.CreateHubProxy("PictionaryHub");
            //await conn.Start();
            proxy.On<string, string>("jugadorDeletedSala", OnjugadorDeleted);
            proxy.On<clsMensaje>("addMensajeToChat", OnaddMensajeToChat);

        }


        #region"Propiedades públicas"
        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand SendMessageCommand { get; }
        public DispatcherTimer DispatcherTimer { get => _dispatcherTimer; set => _dispatcherTimer = value; }
        public clsPartida Partida { get => _partida; set => _partida = value; }
        public string LblTemporizador { get => _lblTemporizador; set => _lblTemporizador = value; }
        public ObservableCollection<clsMensaje> ListadoMensajesChat { get => _listadoMensajesChat; set => _listadoMensajesChat = value; }
        public clsJugador UsuarioPropio { get => _usuarioPropio; set => _usuarioPropio = value; }
        public clsMensaje Mensaje { get => _mensaje; set => _mensaje = value; }

        #endregion


        #region"Command"
        private async void ExecuteGoBackCommand()
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
                navigationService.NavigateTo(ViewModelLocator.ListadoSalas);
                await proxy.Invoke("jugadorHaSalido", _usuarioPropio.Nickname, _partida.NombreSala);
            }
        }

        private void ExecuteSendMessageCommand()
        {
            proxy.Invoke("sendMensaje", Mensaje, _partida.NombreSala);
            _mensaje.Mensaje = "";
            NotifyPropertyChanged("Mensaje");
        }


        private bool CanExecuteSendMessageCommand() => _mensaje != null || _mensaje.Mensaje != "";
        #endregion

        public async void OnjugadorDeleted(string usuario, string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clsJugador jugador;
                try
                {
                    jugador = _partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuario);
                }
                catch (Exception e)
                {
                    jugador = null;
                }

                if (jugador != null)
                {
                    _partida.ListadoJugadores.Remove(jugador);
                    _partida.NotifyPropertyChanged("ListadoJugadores");
                }
            });
        }

        public async void OnaddMensajeToChat(clsMensaje mensaje)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _partida.ListadoMensajes.Add(mensaje);
                _partida.NotifyPropertyChanged("ListadoMensajes");

            });
        }
    }
}
