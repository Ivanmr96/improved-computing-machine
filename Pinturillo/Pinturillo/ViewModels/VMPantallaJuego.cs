using CommonServiceLocator;
using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using Pinturillo.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pinturillo.ViewModels
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
        Frame navigationFrame = Window.Current.Content as Frame;
        private IHubProxy proxy;
        private bool isUltimaPalabraAcertada;
        private string _palabraAMostrar;
        private int pos = 0;
        private bool puedesFuncionar;
        private int tiempoAMostrar;
        private String visible;
        private String turnoJugador;
        public bool hanAcertadoTodos { get; set; }
        #endregion
        public static int TIME_MAX = 90;
        public static int TIME_WAIT = 5;
        public int tiempoEspera { get; set; }
        

        public VMPantallaJuego()
        {
            turnoJugador = " ";
            hanAcertadoTodos = false;
            visible = "Collapsed";
            puedesFuncionar = true;
            this.tiempoAMostrar = 0;
            this.tiempoEspera = TIME_WAIT;
            _partida = new clsPartida();
            _usuarioPropio = new clsJugador();
            this._mensaje = new clsMensaje();
            _mensaje.JugadorQueLoEnvia = new clsJugador();
            this.GoBackCommand = new DelegateCommand(ExecuteGoBackCommand);
            this.SendMessageCommand = new DelegateCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand);
            this.IsUltimaPalabraAcertada = false;
            _timeMax = TIME_MAX;
            this._dispatcherTimer = new DispatcherTimer();
            this._dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            this._dispatcherTimer.Tick += Timer_Tik;
            //this._dispatcherTimer.Start();
            this.LblTemporizador = TIME_MAX.ToString();
            this._palabraAMostrar = "";
            TipoEntradaInkCanvas = CoreInputDeviceTypes.None;


            SignalR();
            
           // proxy.Invoke("comenzarPartidaEnGrupo", _partida);
        }


        private void Timer_Tik(object sender, object e)
        {
            if (_timeMax > 0 )
            {
                if (_timeMax < 88) {
                    turnoJugador = " ";
                    NotifyPropertyChanged("TurnoJugador");
                }
                visible = "Collapsed";
                NotifyPropertyChanged("Visible");
                if (_timeMax % 10 == 0) //si es divisible entre 10 (o sea es 60, 50, 40, 30, 20, 10)
                {
                    //se descubre un caracter
                    if(pos < _partida.PosicionesADescubrir.Count)
                    {
                        this._palabraAMostrar = Util.obtenerPalabraFormateada(_palabraAMostrar, _partida.PalabraEnJuego, _partida.PosicionesADescubrir[pos]);
                        pos++;
                        NotifyPropertyChanged("PalabraAMostrar");
                    }
                   
                }


                if (_timeMax <= 10)
                {
                    _timeMax--;
                    LblTemporizador = "0" + TimeMax.ToString();
                    NotifyPropertyChanged("LblTemporizador");
                }
                else
                {
                    _timeMax--;
                    LblTemporizador = _timeMax.ToString();
                    NotifyPropertyChanged("LblTemporizador");
                }
            } 
            //else
            //{

            //    if (_timeMax == 0 || hanAcertadoTodos)
            //    {
            //        //TODO 
            //        //El contador llega a 0


            //        //Bloquear el chat para todo el mundo en este tiempo
            //        //Ponemos el IsMiTurno a TRUE para que automáticamente se bloquee el input del chat
            //        //(porque esta bindeado con el converter de true to false
            //        visible = "Visible";
            //        NotifyPropertyChanged("Visible");
            //        this.IsMiTurno = true;
            //        NotifyPropertyChanged("IsMiTurno");
            //        //Con esto lo que pasa es que se va a habilitar el inktool bar para todos pero bueno no podrán chatear así que diwa

            //        //se reinicia esto
            //        pos = 0;

            //        //Se muestra la palabra
            //        this._palabraAMostrar = _partida.PalabraEnJuego;
            //        NotifyPropertyChanged("PalabraAMostrar");
            //        tiempoEspera--;
            //        NotifyPropertyChanged("tiempoEspera");

            //        if (tiempoEspera == 0)
            //        {                      
            //            this._dispatcherTimer.Stop();
            //            proxy.Invoke("miContadorHaLlegadoACero", _usuarioPropio.ConnectionID, _partida.NombreSala);
            //        }
            //    }
            //}
            if (_timeMax == 0 || hanAcertadoTodos)
            {
                //TODO 
                //El contador llega a 0

                if (hanAcertadoTodos)
                {
                    this._timeMax = 0;
                    LblTemporizador = _timeMax.ToString();
                    NotifyPropertyChanged("LblTemporizador");
                }


                //Bloquear el chat para todo el mundo en este tiempo
                //Ponemos el IsMiTurno a TRUE para que automáticamente se bloquee el input del chat
                //(porque esta bindeado con el converter de true to false
                visible = "Visible";
                NotifyPropertyChanged("Visible");
                this.IsMiTurno = true;
                NotifyPropertyChanged("IsMiTurno");
                //Con esto lo que pasa es que se va a habilitar el inktool bar para todos pero bueno no podrán chatear así que diwa

                //se reinicia esto
                pos = 0;

                //Se muestra la palabra
                this._palabraAMostrar = _partida.PalabraEnJuego;
                NotifyPropertyChanged("PalabraAMostrar");
                tiempoEspera--;
                NotifyPropertyChanged("tiempoEspera");

                if (tiempoEspera == 0)
                {
                    this._dispatcherTimer.Stop();
                    proxy.Invoke("miContadorHaLlegadoACero", _usuarioPropio.ConnectionID, _partida.NombreSala);
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
            proxy.On<clsPartida>("puntosAdded", OnPuntosAdded);
            proxy.On("todosHanAcertado", OnTodosHanAcertado);
            proxy.On("HaTerminadoLaPartida", OnHaTerminadoLaPartida);

        }

        private async void OnHaTerminadoLaPartida()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _partida.IsJugandose = false;
                ordenarListadoJugadoresPorPuntuacion();
                _partida.NotifyPropertyChanged("IsJugandose");
            });
        }

        private void ordenarListadoJugadoresPorPuntuacion()
        {
            _partida.ListadoJugadores = new ObservableCollection<clsJugador>((from jugador in _partida.ListadoJugadores
                                                                                              orderby jugador.Puntuacion descending
                                                                                              select jugador).ToList<clsJugador>());

            for (int i = 0; i < _partida.ListadoJugadores.Count; i++)
            {
                _partida.ListadoJugadores[i].PosicionFinal = i + 1;
                _partida.ListadoJugadores[i].NotifyPropertyChanged("PosicionFinal");
            }

            _partida.NotifyPropertyChanged("ListadoJugadores");
        }

        private async void OnTodosHanAcertado()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.hanAcertadoTodos = true;
                NotifyPropertyChanged("hanAcertadoTodos");
            });
        }

        #region"Propiedades públicas"
        public DelegateCommand GoBackCommand { get; }
        public int TimeMax { get => _timeMax; set => _timeMax = value; }
        public bool IsMiTurno { get; set; }
        public DelegateCommand SendMessageCommand { get; }
        public DispatcherTimer DispatcherTimer { get => _dispatcherTimer; set => _dispatcherTimer = value; }
        public clsPartida Partida { get => _partida; set => _partida = value; }
        public string LblTemporizador { get => _lblTemporizador; set => _lblTemporizador = value; }
        public string PalabraAMostrar { get => _palabraAMostrar; set => _palabraAMostrar = value; }
        public ObservableCollection<clsMensaje> ListadoMensajesChat { get => _listadoMensajesChat; set => _listadoMensajesChat = value; }
        public clsJugador UsuarioPropio { get => _usuarioPropio; set => _usuarioPropio = value; }
        public clsMensaje Mensaje
        {
            get {
                return _mensaje;                
            }
            set
            {
                _mensaje = value;
            }
        }
        public bool IsUltimaPalabraAcertada { get => isUltimaPalabraAcertada; set => isUltimaPalabraAcertada = value; }
        public CoreInputDeviceTypes TipoEntradaInkCanvas { get; set; }
        public bool PuedesFuncionar { get => puedesFuncionar; set => puedesFuncionar = value; }
        public int TiempoAMostrar { get => tiempoAMostrar; set => tiempoAMostrar = value; }
        public string Visible { get => visible; set => visible = value; }
        public string TurnoJugador { get => turnoJugador; set => turnoJugador = value; }
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
                navigationFrame.Navigate(typeof(ListadoSalas),_usuarioPropio.Nickname);
                proxy.Invoke("jugadorHaSalido", _usuarioPropio.Nickname, _partida.NombreSala);
                puedesFuncionar = false;
                //pararContador();
            }
        }

        private void ExecuteSendMessageCommand()
        {
            if(_mensaje.Mensaje.Contains(_partida.PalabraEnJuego))
            {
                _mensaje.Mensaje = "El usuario " + _usuarioPropio.Nickname + " ha acertado la palabra!";
                //invoke para indicar que ha acertado la palabra

                

                    proxy.Invoke("addPuntosToUser", _usuarioPropio.ConnectionID, _timeMax, _partida.NombreSala);
            }
            proxy.Invoke("sendMensaje", Mensaje, _partida.NombreSala);
            _mensaje.Mensaje = "";
            NotifyPropertyChanged("Mensaje");
            


        }


        private bool CanExecuteSendMessageCommand()
        {
            bool canExecute = false;

            if( (_mensaje.Mensaje != null && _mensaje.Mensaje != "" && IsMiTurno == false) ){
                canExecute = true;
            }
            return canExecute;
        }
        #endregion


        public async void OnPuntosAdded(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                this._partida.ListadoJugadores = obj.ListadoJugadores;
                this._partida.NotifyPropertyChanged("ListadoJugadores");

                //if (PuedesFuncionar)
                //{
                    
                //    PuedesFuncionar = false;
                //}
               
            });
        }

        public async void OnjugadorDeleted(string usuario, string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                clsJugador jugador;

                    jugador = _partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuario);

                if (jugador != null)
                {
                    _partida.ListadoJugadores.Remove(jugador);
                    _partida.NotifyPropertyChanged("ListadoJugadores");
                }

                //if (PuedesFuncionar)
                //{
                   
                //    PuedesFuncionar = false;
                //}
               
            });
        }



        ////Cuando comienza la partida
        //private async void onPartidaComenzada(clsPartida obj)
        //{
        //    await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        this._partida = obj;
        //        NotifyPropertyChanged("Partida");

        //        this._usuarioPropio = obj.ListadoJugadores.First<clsJugador>(x => x.Nickname == _usuarioPropio.Nickname);

        //        //Iniciamos el timer
        //        this._dispatcherTimer.Start();

        //        if (obj.ConnectionIDJugadorActual == _usuarioPropio.ConnectionID)
        //        //es nuestro turno
        //        {
        //            //Habilitar el canvas
        //            TipoEntradaInkCanvas = CoreInputDeviceTypes.Mouse;
        //            NotifyPropertyChanged("TipoEntradaInkCanvas");
        //            //palabra a mostrar será la palabra en juego
        //            this._palabraAMostrar = obj.PalabraEnJuego;
        //            NotifyPropertyChanged("PalabraAMostrar");



        //        }
        //        else
        //        {
        //            //No es nuestro turno

        //            //Deshabilitar el canvas
        //            TipoEntradaInkCanvas = CoreInputDeviceTypes.None;
        //            NotifyPropertyChanged("TipoEntradaInkCanvas");
        //            //palabra a mostrar será  ___ 
        //            this._palabraAMostrar = "*******"; //esto ponerlo con tantos * como letras tenga y tal
        //            NotifyPropertyChanged("PalabraAMostrar");
        //        }

        //    });
        //}

        public async void OnaddMensajeToChat(clsMensaje mensaje)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _partida.ListadoMensajes.Add(mensaje);
                _partida.NotifyPropertyChanged("ListadoMensajes");

            });
        }

        public void textoCambiado() {
            SendMessageCommand.RaiseCanExecuteChanged();
        }

        //public void reiniciarContador() {
        //    pararContador();
        //    this.LblTemporizador = "60";
        //    NotifyPropertyChanged("LblTemporizador");
        //    this._dispatcherTimer.Start();
        //}

        //public void pararContador() {
        //    this._dispatcherTimer.Stop();
        //}
    }
}
