using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pinturillo.ViewModels
{
    public class CrearSalaVM : clsVMBase
    {
        private String _nombreUsuario;
        private clsPartida _partida;
        private String _lblErrorNombreSala; //Si la sala ya existe muestra este label
        private String _lblErrorContrasena; //Si se marca el checkbox de sala privada y no se escribe contrasena
        private DelegateCommand _crearPartida;
        private HubConnection conn;
        private IHubProxy proxy;
        private readonly INavigationService navigationService;
        private String _visible;
        private bool _checkboxChecked;

        private const int NUM_MAX_JUGADORES = 5;

        public List<int> NumJugadores 
        { 
            get 
            {
                List<int> numeros = new List<int>();

                for(int i = 2; i <= NUM_MAX_JUGADORES; i++)
                {
                    numeros.Add(i);
                }

                return numeros;
            }
        }

        public String visible {
            get {
                return _visible;
            }
            set {
                _visible = value;
            }
        }
        public CrearSalaVM(INavigationService navigationService)
        {
            _partida = new clsPartida();
            this.navigationService = navigationService;
            _visible = "Collapsed";
            _checkboxChecked = false;
            SignalR();
        }

        public String NombreUsuario { get => _nombreUsuario; set => _nombreUsuario = value; }
        //public clsPartida Partida { get => _partida; set => _partida = value; }
        public String LblErrorNombreSala { get => _lblErrorNombreSala; set => _lblErrorNombreSala = value; }
        public String LblErrorContrasena { get => _lblErrorContrasena; set => _lblErrorContrasena = value; }

        public DelegateCommand crearPartida
        {
            get
            {
                _crearPartida = new DelegateCommand(CrearCommand_Executed, CrearCommand_CanExecuted);
                return _crearPartida;
            }
        }

        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;
            //await conn.Start();


            proxy.On<clsPartida>("salaCreada", salaCreada);
            //proxy.Invoke("sendSalas");
        }

        private async void salaCreada(clsPartida partida)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.navigationService.NavigateTo(ViewModelLocator.SalaEspera, partida);
            });
        }

        public clsPartida Partida
        {
            get
            {
                return _partida;
            }
            set
            {
                _partida = value;
            }
        }

        public bool CheckboxChecked { get => _checkboxChecked; set => _checkboxChecked = value; }

        private void CrearCommand_Executed()
        {           
            if (_checkboxChecked)
            {
                if (validarFormulario())
                {
                    proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                }
            }
            else {
                if (validarFormularioSoloNombreSala()) {
                    proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                }
            }

            _partida = new clsPartida();
            _partida.NotifyPropertyChanged("NombreSala");
            _partida.NotifyPropertyChanged("Password");
            _partida.NotifyPropertyChanged("NumeroMaximoJugadores");
            _visible = "Collapsed";
            //var hola = "hola";
        }

        //Metodo para validar el formulario si el checkbox no esta marcado
        private bool validarFormularioSoloNombreSala()
        {
            bool valido = true;
            if (String.IsNullOrEmpty(Partida.NombreSala))
            {
                valido = false;
                _lblErrorNombreSala = "Debes introducir uno";
                NotifyPropertyChanged("LblErrorNombreSala");
            }
            else
            {
                _lblErrorNombreSala = "";
                NotifyPropertyChanged("LblErrorNombreSala");
            }
            return valido;
        }

        private bool CrearCommand_CanExecuted()
        {
            return true;
        }

        //Metodo para validar el formulario si el checkbox esta marcado
        public bool validarFormulario() {

            bool valido = true;

            if (String.IsNullOrEmpty(Partida.NombreSala))
            {
                valido = false;
                _lblErrorNombreSala = "Debes introducir uno";
                NotifyPropertyChanged("LblErrorNombreSala");
            }
            else {
                _lblErrorNombreSala = "";
                NotifyPropertyChanged("LblErrorNombreSala");
            }

            if (String.IsNullOrEmpty(_partida.Password)) {
                valido = false;
                _lblErrorContrasena = "Debes introducirla";
                NotifyPropertyChanged("LblErrorContrasena");
            }
            else
            {
                _lblErrorContrasena = "";
                NotifyPropertyChanged("LblErrorContrasena");
            }

            return valido;

        }

        public void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.IsChecked == true)
            {
                _visible = "Visible";
                NotifyPropertyChanged("visible");
                _checkboxChecked = true;
            }
            else
            {
                _visible = "Collapsed";
                NotifyPropertyChanged("visible");
                _checkboxChecked = false;
            }
        }

    }
}
