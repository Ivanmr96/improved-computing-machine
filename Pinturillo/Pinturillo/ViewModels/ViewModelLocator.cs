using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinturillo.ViewModels
{
    public class ViewModelLocator
    {
        public static string MainPage = "MainPage";
        public static string CrearSala = "CrearSalaPage";
        public static string ListadoSalas = "ListadoSalas";
        public static string PantallaJuego = "PantallaJuego";
        public static string SalaEspera = "SalaEspera";
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();
            nav.Configure(MainPage, typeof(MainPage));
            nav.Configure(CrearSala, typeof(CrearSalaPage));
            nav.Configure(ListadoSalas, typeof(ListadoSalas));
            nav.Configure(PantallaJuego, typeof(PantallaJuego));
            nav.Configure(SalaEspera, typeof(SalaEspera));


            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<MainPageVM>();
            SimpleIoc.Default.Register<CrearSalaVM>();
            SimpleIoc.Default.Register<VMListadoSalas>();
            SimpleIoc.Default.Register<VMPantallaJuego>();
            SimpleIoc.Default.Register<SalaEsperaVM>();

        }


        // <summary>
        // Gets the MainPage view model.
        // </summary>
        // <value>
        // The MainPage view model.
        // </value>
        public MainPageVM MainPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageVM>();
            }
        }

        // <summary>
        // Gets the SecondPage view model.
        // </summary>
        // <value>
        // The SecondPage view model.
        // </value>
        public CrearSalaVM CrearSalaInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CrearSalaVM>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public VMListadoSalas ListadoSalasInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMListadoSalas>();
            }
        }

        ///
        public VMPantallaJuego PantallaJuegoInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMPantallaJuego>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SalaEsperaVM SalaEsperaInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SalaEsperaVM>();
            }
        }
    }
}
