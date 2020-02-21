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

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace PinturilloParaPruebas
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }

    }
}
