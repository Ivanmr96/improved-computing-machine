﻿using Pinturillo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pinturillo
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CrearSalaPage : Page
    {
        CrearSalaVM vm { get; set; }
        public CrearSalaPage()
        {
            this.InitializeComponent();

            vm = (CrearSalaVM)DataContext;
        }       
            private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.limpiarCampos();
            vm.limpiarFormulario();
            this.Frame.Navigate(typeof(ListadoSalas), vm.NombreUsuario);
        }        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm.NombreUsuario = (String)e.Parameter;

            base.OnNavigatedTo(e);
        }


    }
}
