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
        private String _lblErrorNumJugadores; //Si no se selecciona un numero de jugadores máximos
        private DelegateCommand _crearPartida;
        private HubConnection conn;
        private IHubProxy proxy;
        Frame navigationFrame = Window.Current.Content as Frame;
        private String _visible;
        private bool _checkboxChecked;
        private bool puedesFuncionar;

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
        public CrearSalaVM()
        {
            puedesFuncionar = true;
            _partida = new clsPartida();
            _visible = "Collapsed";
            _lblErrorNombreSala = "*";
            
            _lblErrorNumJugadores = "*";
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
                if (puedesFuncionar)
                {
                    navigationFrame.Navigate(typeof(SalaEspera), partida);
                    puedesFuncionar = false;
                }
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
        public String LblErrorNumJugadores { get => _lblErrorNumJugadores; set => _lblErrorNumJugadores = value; }

        private void CrearCommand_Executed()
        {           
            if (_checkboxChecked)
            {
                if (validarFormulario())
                {
                   
                        proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                   
                   
                    limpiarCampos();
                }
            }
            else {
                if (validarFormularioSoloNombreSala()) {
                 
                        proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
              
                    
                    limpiarCampos();
                }
            }

            
            //var hola = "hola";
        }

        public void limpiarCampos() {
            _partida = new clsPartida();
            _partida.NotifyPropertyChanged("NombreSala");
            _partida.NotifyPropertyChanged("Password");
            _partida.NotifyPropertyChanged("NumeroMaximoJugadores");
            _visible = "Collapsed";
        }

        //Metodo para validar el formulario si el checkbox no esta marcado
        private bool validarFormularioSoloNombreSala()
        {
            bool valido = true;
            if (String.IsNullOrEmpty(Partida.NombreSala))
            {
                valido = false;
                _lblErrorNombreSala = "*Nombre requerido";
                NotifyPropertyChanged("LblErrorNombreSala");
            }
            else
            {
                _lblErrorNombreSala = "*";
                NotifyPropertyChanged("LblErrorNombreSala");
            }

            if (_partida.NumeroMaximoJugadores == 0)
            {
                valido = false;
                LblErrorNumJugadores = "*Debes seleccionar un numero";
                NotifyPropertyChanged("LblErrorNumJugadores");
            }
            else
            {
                LblErrorNumJugadores = "*";
                NotifyPropertyChanged("LblErrorNumJugadores");
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
                _lblErrorNombreSala = "*Nombre requerido";
                NotifyPropertyChanged("LblErrorNombreSala");
            }
            else {
                _lblErrorNombreSala = "*";
                NotifyPropertyChanged("LblErrorNombreSala");
            }

            if (String.IsNullOrEmpty(_partida.Password)) {
                valido = false;
                _lblErrorContrasena = "*Constraseña requerida";
                NotifyPropertyChanged("LblErrorContrasena");
            }
            else
            {
                _lblErrorContrasena = "*";
                NotifyPropertyChanged("LblErrorContrasena");
            }

            if (_partida.NumeroMaximoJugadores == 0)
            {
                valido = false;
                LblErrorNumJugadores = "*Debes seleccionar un numero";
                NotifyPropertyChanged("LblErrorNumJugadores");
            }
            else
            {
                LblErrorNumJugadores = "*";
                NotifyPropertyChanged("LblErrorNumJugadores");
            }

            return valido;

        }

        public void limpiarFormulario() {
            _lblErrorNombreSala = "*";
            NotifyPropertyChanged("LblErrorNombreSala");
            LblErrorNumJugadores = "*";
            NotifyPropertyChanged("LblErrorNumJugadores");
            _lblErrorContrasena = "";
            NotifyPropertyChanged("LblErrorContrasena");
            _visible = "Collapsed";
            NotifyPropertyChanged("visible");
        }

        public void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.IsChecked == true)
            {
                _visible = "Visible";
                NotifyPropertyChanged("visible");
                _lblErrorContrasena = "*";
                NotifyPropertyChanged("LblErrorContrasena");
                _checkboxChecked = true;
            }
            else
            {
                _visible = "Collapsed";
                NotifyPropertyChanged("visible");
                _lblErrorContrasena = "";
                NotifyPropertyChanged("LblErrorContrasena");
                _checkboxChecked = false;
            }
        }

    }

}
