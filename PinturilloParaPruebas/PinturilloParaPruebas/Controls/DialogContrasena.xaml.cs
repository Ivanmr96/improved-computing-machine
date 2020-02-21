using System;
using System.Collections.Generic;
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

namespace PinturilloParaPruebas.Controls
{
    public sealed partial class DialogContrasena : UserControl
    {
        public DialogContrasena()
        {
            this.InitializeComponent();
        }

        public string Contrasena
        {
            get { return (string)GetValue(ContrasenaProperty); }
            set { SetValue(ContrasenaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Contrasena.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContrasenaProperty =
            DependencyProperty.Register("Contrasena", typeof(string), typeof(DialogContrasena), new PropertyMetadata(String.Empty));


        public ICommand EnterCommand
        {
            get { return (ICommand)GetValue(EnterCommandProperty); }
            set { SetValue(EnterCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterCommandProperty =
            DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(DialogContrasena), new PropertyMetadata(default(ICommand)));



    }
}
