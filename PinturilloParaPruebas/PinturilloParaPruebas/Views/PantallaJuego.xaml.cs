using Microsoft.AspNet.SignalR.Client;
using PinturilloParaPruebas.Models;
using PinturilloParaPruebas.ViewModels;
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

namespace PinturilloParaPruebas
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PantallaJuego : Page
    {

        InkStrokeBuilder builder;
        List<Point> points;
        IReadOnlyList<InkStroke> _added;
        IReadOnlyList<InkStroke> _borrado;
        Line line;
        private HubConnection conn;
        private IHubProxy proxy;

        public static int TIME_MAX = 10;

        VMPantallaJuego viewModel
        {
            get; set;
        }
        public PantallaJuego()
        {
            this.InitializeComponent();
            viewModel = (VMPantallaJuego)this.DataContext;
            /*List<User> items = new List<User>();
            items.Add(new User() { Name = "Angela", Age = 22 });
            items.Add(new User() { Name = "Victor", Age = 20 });
            items.Add(new User() { Name = "Ivan", Age = 23 });
            listadoSalas.ItemsSource = items;*/


            //inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
            //inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;

            inkCanvas.InkPresenter.InputDeviceTypes = viewModel.TipoEntradaInkCanvas;

            InkDrawingAttributes att = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
            att.Color = Color.FromArgb(100, 0, 220, 100);
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(att);

            inkCanvas.InkPresenter.StrokeInput.StrokeContinued += StrokeInput_StrokeContinued;
            inkCanvas.InkPresenter.StrokeInput.StrokeStarted += StrokeInput_StrokeStarted;
            inkCanvas.InkPresenter.StrokeInput.StrokeEnded += StrokeInput_StrokeEnded;
            inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

            builder = new InkStrokeBuilder();
            points = new List<Point>();

            SignalR();
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            _borrado = args.Strokes;
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
            proxy.On<clsPartida>("onPartidaComenzada", onPartidaComenzada);
            proxy.On<clsPartida>("haCambiadoElTurno", onHaCambiadoElTurno);

        }

        //Cuando cambia el turno
        private async void onHaCambiadoElTurno(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                viewModel.Partida = obj;
                viewModel.NotifyPropertyChanged("Partida");

                viewModel.UsuarioPropio = obj.ListadoJugadores.First<clsJugador>(x => x.Nickname == viewModel.UsuarioPropio.Nickname);

                //Iniciamos el timer
                viewModel.TimeMax = TIME_MAX;
                viewModel.NotifyPropertyChanged("TimeMax");
                viewModel.LblTemporizador = TIME_MAX.ToString();
                viewModel.NotifyPropertyChanged("LblTemporizador");

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



                }
                else
                {
                    //No es nuestro turno

                    //Deshabilitar el canvas
                    inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.None;
                    //  NotifyPropertyChanged("TipoEntradaInkCanvas");
                    //palabra a mostrar será  ___ 
                    //viewModel.PalabraAMostrar = "*******"; //esto ponerlo con tantos * como letras tenga y tal

                    viewModel.PalabraAMostrar = new string('*', obj.PalabraEnJuego.Length);
                    viewModel.NotifyPropertyChanged("PalabraAMostrar");                         // NotifyPropertyChanged("PalabraAMostrar");
                }

            });
        }


        //Cuando comienza la partida
        private async void onPartidaComenzada(clsPartida obj)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                viewModel.Partida = obj;
                viewModel.NotifyPropertyChanged("Partida");

                viewModel.UsuarioPropio = obj.ListadoJugadores.First<clsJugador>(x => x.Nickname == viewModel.UsuarioPropio.Nickname);

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



                }
                else
                {
                    //No es nuestro turno

                    //Deshabilitar el canvas
                    // viewModel.TipoEntradaInkCanvas = CoreInputDeviceTypes.None;
                    //  NotifyPropertyChanged("TipoEntradaInkCanvas");
                    //palabra a mostrar será  ___ 
                    // viewModel.PalabraAMostrar = "*******"; //esto ponerlo con tantos * como letras tenga y tal

                    viewModel.PalabraAMostrar = new string('*', obj.PalabraEnJuego.Length);
                    viewModel.NotifyPropertyChanged("PalabraAMostrar");                              // NotifyPropertyChanged("PalabraAMostrar");
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
                    inkpoints.Add(new InkPoint(new Point(p.X, p.Y), 0.5f));
                }

                // Crea el stroke a partir de los inkpoints
                InkStroke stroke = builder.CreateStrokeFromInkPoints(inkpoints, System.Numerics.Matrix3x2.Identity);

                // Copia los atributos de dibujado (color y eso) del canvas original
                InkDrawingAttributes ida = inkCanvas.InkPresenter.CopyDefaultDrawingAttributes();
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
                    punticos.Add(new clsPunto(p.X, p.Y));
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


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var lastPage = Frame.BackStack.Last().SourcePageType;
            
            //if (e.SourcePageType.FullName.Equals("Pinturillo.CrearSalaPage"))
            if (lastPage.FullName.Equals("PinturilloParaPruebas.SalaEspera"))
            {
                if (e.Parameter != null)
                {

                    Tuple<String, clsPartida> partidaConNick = (Tuple<String, clsPartida>)e.Parameter;

                    viewModel.UsuarioPropio.Nickname = partidaConNick.Item1;
                    viewModel.Partida = partidaConNick.Item2;

                    //clsJugador jugadorLider = viewModel.Partida.ListadoJugadores.First<clsJugador>(x => x.IsLider);

                    //viewModel.Partida.ListadoJugadores.

                    //viewModel.UsuarioPropio = jugadorLider;
                    viewModel.Mensaje.JugadorQueLoEnvia = viewModel.UsuarioPropio;
                    proxy.Invoke("comenzarPartidaEnGrupo", viewModel.Partida);
                }
            }
            base.OnNavigatedTo(e);

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
