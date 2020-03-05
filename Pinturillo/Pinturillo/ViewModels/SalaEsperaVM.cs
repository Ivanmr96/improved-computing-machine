using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pinturillo.ViewModels
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
        private bool puedesFuncionar2;
        #endregion

        #region constructor

        /// <summary>
        /// Se ejecuta cuando se cargue la pantalla por primera vez.
        /// 
        /// Realiza configuraciones con la conexión signalR, así como establece los comandos.
        /// </summary>
        public SalaEsperaVM()
        {
            // Aquí obtendría la partida enviada desde la otra ventana
            puedesFuncionar2 = true;
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
            get { return this.usuarioPropio;  }
            set { this.usuarioPropio = value; }
        }

        #endregion

        #region commands

        public DelegateCommand enviarMensaje { get; set; }

        /// <summary>
        /// Podrá ejecuta el comando de enviar mensaje cuando el mensaje no esté vacío ni sea null.
        /// </summary>
        /// <returns></returns>
        public bool enviarMensaje_canExecute() => mensaje.Mensaje != null && mensaje.Mensaje != "";

        /// <summary>
        /// Ejecuta el comando de enviar mensaje, manda el mensaje al servidor y limpia el campo de texto del mensaje
        /// </summary>
        public void enviarMensaje_execute()
        {
            //Mandar el mensaje al servidor
            proxy.Invoke("sendMensaje", mensaje, partida.NombreSala);
            mensaje.Mensaje = "";
            NotifyPropertyChanged("Mensaje");
        }

        public DelegateCommand salir { get; set; }

        /// <summary>
        /// Se realiza cuando se ejecute el comando de salir.
        /// 
        /// Se muestra un diálogo de confirmación para salir.
        /// 
        /// Si el usuario acepta salir, se navega al listado de salas, si no, se cierra el diálogo.
        /// </summary>
        private async void salir_execute()
        {
            //Indica al serivdor que sale.
            
            //await proxy.Invoke("jugadorHaSalido", usuarioPropio, partida.NombreSala);

            //para probar
            //this.navigationService.NavigateTo(ViewModelLocator.ListadoSalas);

            ContentDialog confirmadoCorrectamente = new ContentDialog();
            confirmadoCorrectamente.Title = "Confirmación";
            confirmadoCorrectamente.Content = "¿Seguro que quieres salir?";
            confirmadoCorrectamente.PrimaryButtonText = "Si";
            confirmadoCorrectamente.SecondaryButtonText = "No";
            ContentDialogResult resultado = await confirmadoCorrectamente.ShowAsync();
            if (resultado == ContentDialogResult.Primary)
            {
                //this.Frame.Navigate(typeof(ListadoSalas));
                navigationFrame.Navigate(typeof(ListadoSalas),usuarioPropio);
                await proxy.Invoke("jugadorHaSalido", usuarioPropio, partida.NombreSala);
                puedesFuncionar2 = false;
            }
        }

        public DelegateCommand comenzarPartida { get; set; }

        /// <summary>
        /// El botón de comenzar partida estará habilitado solo para el jugador que sea lider de la partida
        /// </summary>
        /// <returns></returns>
        public bool comenzarPartida_canExecute()
        {
            bool puedeComenzar = false;
            clsJugador jugador;

                jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuarioPropio);

            if (jugador != null){
                if (jugador.IsLider && partida.ListadoJugadores.Count >= 2)
                {
                    puedeComenzar = true;
                }
            }
            //Puede ejecutar si es el lider
            return puedeComenzar;
        }

        /// <summary>
        /// Se realiza cuando se ejecute el comando de comenzar partida.
        /// 
        /// Limpia el listado de mensajes de la partida, indica al servidor que la partida ha comenzado y navega a la pantalla del juego.
        /// </summary>
        public void comenzarPartida_execute()
        {
            //Indica al servidor que la partida va a comenzar.

            //if (puedesFuncionar2)
            //{

            //}
            if (puedesFuncionar2)
            {
                partida.IsJugandose = true;
                partida.ListadoMensajes = new System.Collections.ObjectModel.ObservableCollection<clsMensaje>();
                proxy.Invoke("empezarPartida", partida.NombreSala);
                Tuple<String, clsPartida> partidaConNick = new Tuple<string, clsPartida>(usuarioPropio, partida);
                navigationFrame.Navigate(typeof(PantallaJuego), partidaConNick);
                puedesFuncionar2 = false;
            }
        }

        #endregion

        /// <summary>
        /// Realiza las configuraciones de la conexion con singalR para esta pantalla.
        /// 
        /// Registra los métodos de callback con el servidor signalR:
        ///     - Cuando se haya escrito un mensaje nuevo en el chat
        ///     - Cuando se haya unido un jugador a la partida
        ///     - Cuando se haya ido un jugador
        ///     - Cuando la partida haya comenzado
        ///     - Cuando se haya nombrado un nuevo lider para la partida
        /// </summary>
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
            proxy.On("empezarPartida",OnempezarPartidaAsync);
            proxy.On("nombrarComoLider", OnnombrarComoLider);

        }

        /// <summary>
        /// Cuando se empiece la partida, se marcará como jugándose, se limpiará el listado de mensajes y se navegará a la pantalla del juego
        /// </summary>
        private async void OnempezarPartidaAsync()
        {
            //Ir a la pantalla de juego
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (puedesFuncionar2)
                {
                    this.puedesFuncionar2 = false;
                    partida.IsJugandose = true;
                    partida.ListadoMensajes = new System.Collections.ObjectModel.ObservableCollection<clsMensaje>();
                    Tuple<String, clsPartida> partidaConNick = new Tuple<string, clsPartida>(usuarioPropio, partida);
                    navigationFrame.Navigate(typeof(PantallaJuego), partidaConNick);
                }
                
            });
        }

        /// <summary>
        /// Se ejecuta cuando el servidor indica que este cliente es el nuevo jugador lider de la partida.
        /// 
        /// Se establece como lider y e indica al servidor que es el nuevo lider.
        /// </summary>
        private async void OnnombrarComoLider()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                    clsJugador jugador;

                    jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuarioPropio);


                    if (jugador != null)
                    {
                        jugador.IsLider = true;
                        comenzarPartida.RaiseCanExecuteChanged();

                    //Hay que indicarle al servidor cuál es el nuevo lider
                     proxy.Invoke("habemusNuevoLider", jugador.Nickname, partida.NombreSala);
                }
                
               
            });
        }

        /// <summary>
        /// Cuando un jugador se haya unido a la partida, lo añade al listado de jugadores.
        /// </summary>
        /// <param name="jugador"></param>
        /// <param name="game"></param>
        private async void jugadorAdded(clsJugador jugador, clsPartida game)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                    if (partida != null)
                    {
                    //El problema es que aquí no tienes el nombre de la partida
                    if(this.partida.NombreSala == game.NombreSala)
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
                }
            });
        }

        /// <summary>
        /// Cuando se haya ido un jugador de la partida, lo borra del listado de jugadores.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="nombreSala"></param>
        public async void OnjugadorDeleted(string usuario, string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                    clsJugador jugador;
                    jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuario);


                    if (jugador != null)
                    {
                        partida.ListadoJugadores.Remove(jugador);
                        partida.NotifyPropertyChanged("ListadoJugadores");
                        comenzarPartida.RaiseCanExecuteChanged();
                    }
                 
               
            });
        }

        /// <summary>
        /// Cuando se haya escrito un nuevo mensaje en el chat, lo añade al listado de mensajes.
        /// </summary>
        /// <param name="mensaje"></param>
        public async void  OnaddMensajeToChat (clsMensaje mensaje)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                    partida.ListadoMensajes.Add(mensaje);
                    partida.NotifyPropertyChanged("ListadoMensajes");
              
                

            });
        }

        public void textoCambiado()
        {
            enviarMensaje.RaiseCanExecuteChanged();
        }
    }
}
