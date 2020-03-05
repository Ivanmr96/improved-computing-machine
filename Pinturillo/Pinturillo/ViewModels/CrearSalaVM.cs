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

        /// <summary>
        /// Establece el estado de la pantalla de creación de sala.
        /// </summary>
        public CrearSalaVM()
        {
            PuedesFuncionar = true;
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

        /// <summary>
        /// Establece la configuración de la conexión con signalR.
        /// 
        /// Establece los métodos de callback que se ejecutarán cuando haya una sala creada y cunado se haya comprobado que el nombre de la sala existe.
        /// </summary>
        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;
            //await conn.Start();


            proxy.On<clsPartida>("salaCreada", salaCreada);
            proxy.On<bool>("nombreSalaComprobado", OnNombreSalaComprobado);
            //proxy.Invoke("sendSalas");
        }

        /// <summary>
        /// Se ejecutará cuando el servidor indique al cliente que se ha comprobado el nombre de la sala.
        /// 
        /// Si el nombre es válido (único), se pedirá al servidor que añada la partida y se limpiará el formulario.
        /// Si no es válido, mostrará un mensaje de error.
        /// </summary>
        /// <param name="isNombreSalaUnico">Indica si el nombre de la sala es único o no.</param>
        private async void OnNombreSalaComprobado(bool isNombreSalaUnico)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (isNombreSalaUnico)
                {
                    _lblErrorNombreSala = "";
                  
                    proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                    limpiarCampos();
                }
                else
                {
                    _lblErrorNombreSala = "* Ese nombre ya existe, prueba otro.";
                    
                    
                }
                NotifyPropertyChanged("LblErrorNombreSala");
             
               
            });
               
        }
        
        /// <summary>
        /// Indica que la sala ha sido creada correctamente.
        /// 
        /// Navegará hacia la sala de espera de la sala.
        /// </summary>
        /// <param name="partida">La partida que se ha creado</param>
        private async void salaCreada(clsPartida partida)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (PuedesFuncionar)
                {
                    navigationFrame.Navigate(typeof(SalaEspera), partida);
                    PuedesFuncionar = false;
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
        public bool PuedesFuncionar { get => puedesFuncionar; set => puedesFuncionar = value; }

        /// <summary>
        /// Se realiza cuando se ejecute el comando de crear partida.
        /// 
        /// Se validará el formulario de la pantalla, si es correcto se comprobará el nombre de la sala (que posteriormente hará que se cree la partida asi es válido el nombre).
        /// </summary>
        private void CrearCommand_Executed()
        {
            
            NotifyPropertyChanged("Partida");
            _partida.NotifyPropertyChanged("NombreSala");
            _partida.NotifyPropertyChanged("NumeroMaximoJugadores");
            _partida.NotifyPropertyChanged("Password");
            NotifyPropertyChanged("Partida");
            if (_checkboxChecked)
            {
                if (validarFormulario())
                {
                    
                    proxy.Invoke("comprobarNombreSalaUnico", _partida.NombreSala);
                        //proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                    //limpiarCampos();
                }
            }
            else {
                
                if (validarFormularioSoloNombreSala()) {

                    
                    proxy.Invoke("comprobarNombreSalaUnico", _partida.NombreSala);
                    //proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
                    //limpiarCampos();
                }
            }

            
            //var hola = "hola";
        }

        /// <summary>
        /// Limpia los campos del formulario de la creación de sala
        /// </summary>
        public void limpiarCampos() {
            _partida = new clsPartida();
            _partida.NotifyPropertyChanged("NombreSala");
            _partida.NotifyPropertyChanged("Password");
            _partida.NotifyPropertyChanged("NumeroMaximoJugadores");
            
            _visible = "Collapsed";
        }

        /// <summary>
        /// Valida el formulario de creación de sala sin contraseña
        /// </summary>
        /// <returns>true si es válido, false si no lo es</returns>
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

        /// <summary>
        /// Valida el formulario de la creación de sala cuando tiene contraseña
        /// </summary>
        /// <returns>true si es válido, false si no lo es</returns>
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

        /// <summary>
        /// Limpia el formulario
        /// </summary>
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

        /// <summary>
        /// Se ejecutará cuando el checkbox de la contraseña se habilite o deshabilite.
        /// 
        /// Se mostrará el campo de la contraseña si está habilitado o se ocultará si no lo está.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
