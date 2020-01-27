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
        public const string FirstPageKey = "MainPage";
        public const string SecondPageKey = "CrearSalaPage";
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();
            nav.Configure(FirstPageKey, typeof(MainPage));
            nav.Configure(SecondPageKey, typeof(CrearSalaPage));

            //Register your services used here
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<MainPageVM>();
            SimpleIoc.Default.Register<CrearSalaVM>();


        }


        // <summary>
        // Gets the FirstPage view model.
        // </summary>
        // <value>
        // The FirstPage view model.
        // </value>
        public MainPageVM FirstPageInstance
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
        public CrearSalaVM SecondPageInstance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CrearSalaVM>();
            }
        }
    }
}
