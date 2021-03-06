﻿using Pinturillo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace Pinturillo
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageVM viewModel { get; }
        public MainPage()
        {
            //var view = DisplayInformation.GetForCurrentView();

            //// Get the screen resolution (APIs available from 14393 onward).
            //var resolution = new Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);

            //// Calculate the screen size in effective pixels. 
            //// Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
            //var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
            //var bounds = new Size(resolution.Width / scale, resolution.Height / scale);

            //ApplicationView.PreferredLaunchViewSize = new Size(bounds.Width, bounds.Height);
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;


            this.InitializeComponent();
            viewModel = (MainPageVM)DataContext;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }

        /// <summary>
        /// Evento que se ejecuta cuando el usuario pulse el boton Enter cuando el foco esté en el textbox del nickname.
        /// 
        /// Si el comando de entrar al juego está habilitado, entra al juego, si no, no hace nada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtboxNick_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (viewModel.EntrarAlJuego_CanExecuted())
                {
                    viewModel.EntrarAlJuego_Executed();
                }

            }
        }
        
        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Evento asociado al click en el icono de información.
        /// 
        /// Muestra un diálogo con la información de los créditos de la aplicación.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            viewModel.VisibilidadDialog = true;
            viewModel.NotifyPropertyChanged("VisibilidadDialog");
        }
    }
}
