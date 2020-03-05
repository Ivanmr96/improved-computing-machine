using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using Pinturillo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pinturillo
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PantallaJuego : Page
    {

        public static int TIME_MAX = 90;
        public static int TIME_WAIT = 5;

        InkStrokeBuilder builder;
        List<Point> points;
        IReadOnlyList<InkStroke> _added;
        Line line;
        private HubConnection conn;
        private IHubProxy proxy;

        VMPantallaJuego viewModel { get;
        }

        /// <summary>
        /// Cuando se crea la pantalla:
        /// Se establece la configuración para el canvas, indicando el tipo de entrada para el canvas (ratón y táctil)
        /// Y registrando los métodos que se ejecutarán cuando:
        ///     - Cuando se empiece a pintar
        ///     - Cuando se continue pintando
        ///     - Cuando se termine de pintar
        /// </summary>
        public PantallaJuego()
        {
            this.InitializeComponent();
            viewModel = (VMPantallaJuego)this.DataContext;
            //inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
            //inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;

            inkCanvas.InkPresenter.InputDeviceTypes = viewModel.TipoEntradaInkCanvas;

            InkDrawingAttributes att = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            att.Color = Windows.UI.Colors.Black;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(att);

            inkCanvas.InkPresenter.StrokeInput.StrokeContinued += StrokeInput_StrokeContinued;
            inkCanvas.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkCanvas.InkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
            inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

            builder = new InkStrokeBuilder();
            points = new List<Point>();
            SignalR();
        }

        /// <summary>
        /// Registra los métodos de callback que se ejecutaran cuando el servidor indique:
        /// - Que se ha recibido un trazo para pintar en el canvas
        /// - Que ha cambiado el turno
        /// - Que la partida ha comenzando
        /// - Que el canvas ha sido limpiado
        /// </summary>
        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;
            //conn = new HubConnection("http://localhost:11111/");
            //proxy = conn.CreateHubProxy("PictionaryHub");
            //await conn.Start();

            proxy.On<List<clsPunto>>("mandarStroke", onStrokeReceived);
            proxy.On<clsPartida>("haCambiadoElTurno", onHaCambiadoElTurno);
            proxy.On<clsPartida>("onPartidaComenzada", onPartidaComenzada);
            proxy.On("borrarCanvas", borrarCanvas);
        }

        /// <summary>
        /// Se ejecutará cuando el servidor indique se ha cambiado el turno.
        /// 
        /// Se reiniciará el temporizador
        /// Se limpiará el canvas, y si es el turno de este cliente se habilitará para que se pueda pintar en el, si no, se deshabilitará.
        /// 
        /// Se establecerá la nueva palabra a acertar, mostrándola oculta con guiones en la interfaz.
        /// </summary>
        /// <param name="obj">La partida que se está jugando</param>
        private async void onHaCambiadoElTurno(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (viewModel.PuedesFuncionar)
                {
                    if (obj.ListadoJugadores != null && viewModel.UsuarioPropio.Nickname != null)
                    {
                        viewModel.Partida = obj;
                        viewModel.NotifyPropertyChanged("Partida");
                        viewModel.HaAcertado = false;
                        viewModel.UsuarioPropio = obj.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.Nickname == viewModel.UsuarioPropio.Nickname);
                        viewModel.hanAcertadoTodos = false;
                        //Iniciamos el timer
                        viewModel.TimeMax = TIME_MAX;
                        viewModel.NotifyPropertyChanged("TimeMax");
                        viewModel.LblTemporizador = TIME_MAX.ToString();
                        viewModel.NotifyPropertyChanged("LblTemporizador");
                        viewModel.tiempoEspera = TIME_WAIT;
                        viewModel.DispatcherTimer.Start();

                        //Se limpia el canvas
                        inkCanvas.InkPresenter.StrokeContainer.Clear();

                        if (obj.ConnectionIDJugadorActual == viewModel.UsuarioPropio.ConnectionID)
                        //es nuestro turno
                        {
                            //Habilitar el canvas
                            // viewModel.TipoEntradaInkCanvas = CoreInputDeviceTypes.Mouse;
                            if (viewModel.TimeMax > 88)
                            {
                                txtTurno.Visibility = Visibility.Visible;
                                viewModel.TurnoJugador = "ES TU TURNO";
                                viewModel.NotifyPropertyChanged("TurnoJugador");
                            }

                            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse;
                            //NotifyPropertyChanged("TipoEntradaInkCanvas");
                            //palabra a mostrar será la palabra en juego
                            viewModel.PalabraAMostrar = obj.PalabraEnJuego;
                            viewModel.NotifyPropertyChanged("PalabraAMostrar");
                            viewModel.IsMiTurno = true;
                            viewModel.NotifyPropertyChanged("IsMiTurno");


                        }
                        else
                        {
                            //No es nuestro turno
                            if (viewModel.TimeMax >= 88)
                            {
                                txtTurno.Visibility = Visibility.Visible;
                                clsJugador jugador = viewModel.Partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == viewModel.Partida.ConnectionIDJugadorActual);
                                viewModel.TurnoJugador = "ES EL TURNO DE " + jugador.Nickname;
                                viewModel.NotifyPropertyChanged("TurnoJugador");
                            }

                            //Deshabilitar el canvas
                            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;
                            //InkDrawingAttributes att = new InkDrawingAttributes();
                            //att.Color = Windows.UI.Colors.Black;
                            //inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(att);
                            ballpointpen.SelectedBrushIndex = 0;
                            inkToolbar.ActiveTool = null;

                            //  NotifyPropertyChanged("TipoEntradaInkCanvas");
                            //palabra a mostrar será  ___ 
                            // viewModel.PalabraAMostrar = "*******"; //esto ponerlo con tantos * como letras tenga y tal

                            String[] arrayPalabras = obj.PalabraEnJuego.Split(' ');
                            String palabraResuelta = "";
                            foreach (var palabra in arrayPalabras)
                            {
                                palabraResuelta += new String('-', palabra.Length);
                                palabraResuelta += " ";
                            }
                            viewModel.PalabraAMostrar = palabraResuelta;
                            viewModel.IsMiTurno = false;
                            viewModel.NotifyPropertyChanged("IsMiTurno");
                            viewModel.NotifyPropertyChanged("PalabraAMostrar");                         // NotifyPropertyChanged("PalabraAMostrar");
                        }
                    }
                }
            });
        }


        /// <summary>
        /// Se ejecutará cuando el servidor indique que la partido ha comenzado.
        /// 
        /// Se inicia el temporizador para el primer turno.
        /// 
        /// Si es el turno del cliente, se habilita el canvas, si no, se deshabilita.
        /// 
        /// Se establecerá la nueva palabra a acertar, mostrándola oculta con guiones en la interfaz.
        /// </summary>
        /// <param name="obj">La partida que acaba de comenzar</param>
        private async void onPartidaComenzada(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (viewModel.PuedesFuncionar)
                {
                    if(obj.ListadoJugadores!= null && viewModel.UsuarioPropio.Nickname != null)
                    {
                        obj.ListadoMensajes = new System.Collections.ObjectModel.ObservableCollection<clsMensaje>();

                        viewModel.Partida = obj;
                        viewModel.NotifyPropertyChanged("Partida");
                        viewModel.hanAcertadoTodos = false;
                        viewModel.UsuarioPropio = obj.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.Nickname == viewModel.UsuarioPropio.Nickname);

                        //Iniciamos el timer
                        viewModel.DispatcherTimer.Start();

                        if (obj.ConnectionIDJugadorActual == viewModel.UsuarioPropio.ConnectionID)
                        //es nuestro turno
                        {
                            if (viewModel.TimeMax >= 88)
                            {
                                txtTurno.Visibility = Visibility.Visible;
                                viewModel.TurnoJugador = "ES TU TURNO";
                                viewModel.NotifyPropertyChanged("TurnoJugador");
                            }

                            //Habilitar el canvas
                            // viewModel.TipoEntradaInkCanvas = CoreInputDeviceTypes.Mouse;
                            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse;
                            //NotifyPropertyChanged("TipoEntradaInkCanvas");
                            //palabra a mostrar será la palabra en juego
                            viewModel.PalabraAMostrar = obj.PalabraEnJuego;
                            viewModel.NotifyPropertyChanged("PalabraAMostrar");
                            viewModel.IsMiTurno = true;
                            viewModel.NotifyPropertyChanged("IsMiTurno");


                        }
                        else
                        {
                            //No es nuestro turno
                            if (viewModel.TimeMax >= 88)
                            {
                                txtTurno.Visibility = Visibility.Visible;
                                clsJugador jugador = viewModel.Partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.ConnectionID == viewModel.Partida.ConnectionIDJugadorActual);
                                viewModel.TurnoJugador = "ES EL TURNO DE " + jugador.Nickname;
                                viewModel.NotifyPropertyChanged("TurnoJugador");
                            }
                            //Deshabilitar el canvas
                            // viewModel.TipoEntradaInkCanvas = CoreInputDeviceTypes.None;
                            inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;
                            //  NotifyPropertyChanged("TipoEntradaInkCanvas");
                            //palabra a mostrar será  ___ 
                            //viewModel.PalabraAMostrar = "*******"; //esto ponerlo con tantos * como letras tenga y tal
                            viewModel.IsMiTurno = false;
                            viewModel.NotifyPropertyChanged("IsMiTurno");
                            String[] arrayPalabras = obj.PalabraEnJuego.Split(' ');
                            String palabraResuelta = "";
                            foreach (var palabra in arrayPalabras)
                            {
                                palabraResuelta += new String('-',palabra.Length);
                                palabraResuelta += " ";
                            }
                            viewModel.PalabraAMostrar = palabraResuelta;
                            viewModel.NotifyPropertyChanged("PalabraAMostrar");
                        }
                        //viewModel.PuedesFuncionar = false;
                        //viewModel.NotifyPropertyChanged("PuedesFuncionar");
                    }
                   
                }

              
            });
        }

        /// <summary>
        /// Se ejecutará cuando se haya recibido una nueva traza pintada en el canvas.
        /// 
        /// Se cogerá el listado de puntos recibidos y se pintarán en el canvas para representar la traza pintada por otro usuario.
        /// </summary>
        /// <param name="puntos"></param>
        private async void onStrokeReceived(List<clsPunto> puntos)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                    List<InkPoint> inkpoints = new List<InkPoint>();

                    foreach (clsPunto p in puntos)
                    {
                        inkpoints.Add(new InkPoint(new Point(p.X, p.Y),0.5f));
                            
                    }

                    // Crea el stroke a partir de los inkpoints
                    InkStroke stroke = builder.CreateStrokeFromInkPoints(inkpoints, System.Numerics.Matrix3x2.Identity);

                    // Copia los atributos de dibujado (color y eso) del canvas original
                    InkDrawingAttributes ida = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                    ida.Color = puntos[0].Color;
                    ida.Size = puntos[0].Size;
                    
                    stroke.DrawingAttributes = ida;

                    inkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
                    
                

            });
        }


        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            _added = args.Strokes;
        }

        /// <summary>
        /// Cuando termine de pintar el usuario, vacía el listado de puntos para que esté listo para la siguiente traza que se pinte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void StrokeInput_StrokeEnded(InkStrokeInput sender, PointerEventArgs args)
        {
            points = new List<Point>();
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
           

            //builder.BeginStroke(args.CurrentPoint);

            //line = new Line();
            //line.X1 = args.CurrentPoint.RawPosition.X;
            //line.Y1 = args.CurrentPoint.RawPosition.Y;
            //line.X2 = args.CurrentPoint.RawPosition.X;
            //line.Y2 = args.CurrentPoint.RawPosition.Y;

            //line.Stroke = new SolidColorBrush(Colors.Purple);
            //line.StrokeThickness = 4; 
        }

        /// <summary>
        /// Cuando el usuario realice una traza pintada, se convertira dicha traza en puntos y se mandarán al servidor para que el restro de jugadores la reciban y la procesen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void StrokeInput_StrokeContinued(InkStrokeInput sender, PointerEventArgs args)
        {
            // Coge el punto por el que ha pasado el cursor pulsado
            Point point = new Point(args.CurrentPoint.Position.X, args.CurrentPoint.Position.Y);

            // Lo añade a la lista de puntos
            points.Add(point);

            // Si la lista tiene más de un punto
            if (points.Count > 1)
            {
                // Creau una lista de InkPoints a partir de la lista de puntos, necesaria para hacer el stroke
                List<InkPoint> inkpoints = new List<InkPoint>();

                foreach (Point p in points)
                {
                    inkpoints.Add(new InkPoint(p, 0.5f));
                }

                // Crea el stroke a partir de los inkpoints
                InkStroke stroke = builder.CreateStrokeFromInkPoints(inkpoints, System.Numerics.Matrix3x2.Identity);

                // Copia los atributos de dibujado (color y eso) del canvas original
                InkDrawingAttributes ida = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
                
                stroke.DrawingAttributes = ida;

                // Le añadae el stroke creado al canvas de copia
                //canvas2.InkPresenter.StrokeContainer.AddStroke(stroke);

                List<clsPunto> punticos = new List<clsPunto>();

                foreach (Point p in points)
                {
                    punticos.Add(new clsPunto(p.X, p.Y,ida.Color, ida.Size));
                }

                if (conn.State == ConnectionState.Connected)
                {
                    proxy.Invoke("strokeDraw", punticos, viewModel.Partida.NombreSala);
                }

                // Guarda el ultimo punto
                Point ultimo = points.Last<Point>();

                // Vacía la lista de puntos y le añade el ultimo del anterior stroke, para que el dibujado no de "saltos"
                points = new List<Point>();
                points.Add(ultimo);
            }
        }


        /// <summary>
        /// Se ejecuta cuando se ha navegado a esta pantalla.
        /// 
        /// Obtiene de la anterior pantalla tanto la partida como el nombre de usuario, necesarios para poder preparar esta pantalla para el inicio de la partida.
        /// 
        /// Al acabar, indica al servidor que ya ha navegado.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;
            //Frame.BackStack.Clear();
            //if (e.SourcePageType.FullName.Equals("Pinturillo.CrearSalaPage"))
            if (lastPage.FullName.Equals("Pinturillo.SalaEspera"))
            {
                if (e.Parameter != null)
                {
                    Tuple<String,clsPartida> partidaConNick = (Tuple<String, clsPartida>)e.Parameter;

                    //viewModel.UsuarioPropio.Nickname = partidaConNick.Item1;
                    viewModel.Partida = partidaConNick.Item2;
                    viewModel.NotifyPropertyChanged("Partida");

                    clsJugador jugadorLider = viewModel.Partida.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.Nickname == partidaConNick.Item1);

                    //viewModel.Partida.ListadoJugadores.

                    viewModel.UsuarioPropio = jugadorLider;
                    viewModel.Mensaje.JugadorQueLoEnvia = viewModel.UsuarioPropio;


                    //Este invoke solo lo puede emitir el lider (que si no el servidor recibe 5 llamadas pa lo mismo)
                    //Pero no sé como hacer que solo se invoke una vez ya que aun no está puesto el connectionID del jugador actual en la partida xD
                    //De momento lo he apañao en el servidor poniendo que si es null la partida no haga nada 

                    //if (viewModel.UsuarioPropio.IsLider && viewModel.PuedesFuncionar)
                    //    //Esto es un apaño, tengo que cambiarlo para que haga el invoke "el primero que no sea null" (por si el usuario 0 se habia salido o algo asi)
                    //{
                    //    proxy.Invoke("comenzarPartidaEnGrupo", viewModel.Partida);
                    //}
                        
                }
            }


            //Notificar al servidor que ya he navegado
            proxy.Invoke("yaHeNavegado", viewModel.Partida.NombreSala);

            base.OnNavigatedTo(e);

        }

        /// <summary>
        /// Evento asociado al click del botón de "borrar todas las entradas".
        /// 
        /// Indica al servidor que el canvas ha sido borrado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void InkToolbar_EraseAllClicked(InkToolbar sender, object args)
        {
            proxy.Invoke("borrarCanvas", viewModel.Partida.NombreSala);
        }

        /// <summary>
        /// Limpia el canvas, borrando todas las trazas.
        /// </summary>
        private async void borrarCanvas()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                inkCanvas.InkPresenter.StrokeContainer.Clear();
            });
        }

        /// <summary>
        /// Evento asociado al click del botón del borrador.
        /// 
        /// limpia el canvas e indica al servidor de que el canvas ha sido borrado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkToolbarEraserButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Cuando pulsas el botón de borrar, se borra todo
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            proxy.Invoke("borrarCanvas", viewModel.Partida.NombreSala);

        }

        /// <summary>
        /// Evento que se ejecuta cuando el usuario pulsa el boton Enter.
        /// 
        /// Si el comando de mandar mensaje se puede ejecutar, se manda el mensaje escrito.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputMensajes_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (viewModel.CanExecuteSendMessageCommand())
                {
                    viewModel.ExecuteSendMessageCommand();
                }
                
            }
        }
    }
}
