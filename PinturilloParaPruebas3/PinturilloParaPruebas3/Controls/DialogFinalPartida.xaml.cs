using PinturilloParaPruebas3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace PinturilloParaPruebas3.Controls
{
    public sealed partial class DialogFinalPartida : UserControl
    {
        public DialogFinalPartida()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<clsJugador> ListadoJugadores
        {
            get { return (ObservableCollection<clsJugador>)GetValue(ListadoJugadoresProperty); }
            set { SetValue(ListadoJugadoresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListadoJugadores.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListadoJugadoresProperty =
            DependencyProperty.Register("ListadoJugadores", typeof(ObservableCollection<clsJugador>), typeof(DialogFinalPartida), new PropertyMetadata(default(clsJugador)));



        public DelegateCommand CommandSalir
        {
            get { return (DelegateCommand)GetValue(CommandSalirProperty); }
            set { SetValue(CommandSalirProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandSalirProperty =
            DependencyProperty.Register("CommandSalir", typeof(DelegateCommand), typeof(DialogFinalPartida), new PropertyMetadata(default(DelegateCommand)));




    }
}
