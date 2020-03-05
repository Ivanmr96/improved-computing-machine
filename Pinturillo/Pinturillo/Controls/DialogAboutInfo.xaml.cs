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

namespace Pinturillo.Controls
{
    public sealed partial class DialogAboutInfo : UserControl
    {


        public ICommand SalirCommand
        {
            get { return (ICommand)GetValue(SalirCommandProperty); }
            set { SetValue(SalirCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SalirCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SalirCommandProperty =
            DependencyProperty.Register("SalirCommand", typeof(ICommand), typeof(DialogAboutInfo), 
                new PropertyMetadata(default(ICommand)));



        public DialogAboutInfo()
        {
            this.InitializeComponent();
        }
    }
}
