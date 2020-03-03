using Microsoft.AspNet.SignalR.Client;
using PinturilloParaPruebas3.Models;
using PinturilloParaPruebas3.ViewModels;
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

namespace PinturilloParaPruebas3
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PantallaJuego : Page
    {

        public static int TIME_MAX = 10;
        public static int TIME_WAIT = 3;

        InkStrokeBuilder builder;
        List<Point> points;
        IReadOnlyList<InkStroke> _added;
        Line line;
        private HubConnection conn;
        private IHubProxy proxy;

        VMPantallaJuego viewModel { get;
        }
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

        //Cuando cambia el turno
        private async void onHaCambiadoElTurno(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                if(obj.ListadoJugadores != null && viewModel.UsuarioPropio.Nickname != null)
                {
                    viewModel.Partida = obj;
                    viewModel.NotifyPropertyChanged("Partida");

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
                            palabraResuelta += new String('*', palabra.Length);
                            palabraResuelta += " ";
                        }
                        viewModel.PalabraAMostrar = palabraResuelta;
                        viewModel.IsMiTurno = false;
                        viewModel.NotifyPropertyChanged("IsMiTurno");
                        viewModel.NotifyPropertyChanged("PalabraAMostrar");                         // NotifyPropertyChanged("PalabraAMostrar");
                    }
                }
            });
        }




        //Cuando comienza la partida
        private async void onPartidaComenzada(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                if (viewModel.PuedesFuncionar)
                {
                    if(obj.ListadoJugadores!= null && viewModel.UsuarioPropio.Nickname != null)
                    {
                        viewModel.Partida = obj;
                        viewModel.NotifyPropertyChanged("Partida");
                        viewModel.hanAcertadoTodos = false;
                        viewModel.UsuarioPropio = obj.ListadoJugadores.FirstOrDefault<clsJugador>(x => x.Nickname == viewModel.UsuarioPropio.Nickname);

                        //Iniciamos el timer
                        viewModel.DispatcherTimer.Start();

                        if (obj.ConnectionIDJugadorActual == viewModel.UsuarioPropio.ConnectionID)
                        //es nuestro turno
                        {
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
                                palabraResuelta += new String('*',palabra.Length);
                                palabraResuelta += " ";
                            }
                            viewModel.PalabraAMostrar = palabraResuelta;
                            viewModel.NotifyPropertyChanged("PalabraAMostrar");
                        }
                        viewModel.PuedesFuncionar = false;
                        viewModel.NotifyPropertyChanged("PuedesFuncionar");
                    }
                   
                }

              
            });
        }

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
        /*
        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }*/

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;
            //Frame.BackStack.Clear();
            //if (e.SourcePageType.FullName.Equals("PinturilloParaPruebas3.CrearSalaPage"))
            if (lastPage.FullName.Equals("PinturilloParaPruebas3.SalaEspera"))
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

        private void InkToolbar_EraseAllClicked(InkToolbar sender, object args)
        {
            proxy.Invoke("borrarCanvas", viewModel.Partida.NombreSala);
        }

        private async void borrarCanvas()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                inkCanvas.InkPresenter.StrokeContainer.Clear();
            });
        }

        private void InkToolbarEraserButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Cuando pulsas el botón de borrar, se borra todo
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            proxy.Invoke("borrarCanvas", viewModel.Partida.NombreSala);

        }





        //// Update ink stroke color for new strokes.
        //private void OnPenColorChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (inkCanvas != null)
        //    {
        //        InkDrawingAttributes drawingAttributes =
        //            inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();

        //        string value = ((ComboBoxItem)PenColor.SelectedItem).Content.ToString();

        //        switch (value)
        //        {
        //            case "Black":
        //                drawingAttributes.Color = Windows.UI.Colors.Black;
        //                break;
        //            case "Red":
        //                drawingAttributes.Color = Windows.UI.Colors.Red;
        //                break;
        //            default:
        //                drawingAttributes.Color = Windows.UI.Colors.Black;
        //                break;
        //        };

        //        inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);
        //    }
        //}

    }
}
