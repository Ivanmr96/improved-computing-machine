using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
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
    public class MainPageVM : clsVMBase
    {
        private String _nick;
        private DelegateCommand _entrarAlJuegoCommand;
        private HubConnection conn;
        private IHubProxy proxy;
    


        public bool VisibilidadDialog { get; set; }
        public string visibilidadMensajeError { get; set; }
        Frame navigationFrame = Window.Current.Content as Frame;

        public DelegateCommand SalirCommand{get;set;}

        /// <summary>
        /// Se ejecutárá cuando se cree la pantalla por primera vez.
        /// 
        /// Establece el comportamiento de los comandos y de la configuración de la conexión con signalR
        /// </summary>
        public MainPageVM() {
            
            visibilidadMensajeError = "Collapsed";
            SalirCommand = new DelegateCommand(SalirCommandExecute);
            SignalR();
        }

        /// <summary>
        /// Se realiza cuando se ejecuta el comando de salir del diálogo.
        /// 
        /// Cierra el dialogo de créditos
        /// </summary>
        private void SalirCommandExecute()
        {
            this.VisibilidadDialog = false;
            NotifyPropertyChanged("VisibilidadDialog");
        }

        /// <summary>
        /// Establece la configuración con el servidor signalR.
        /// 
        /// Realiza la conexión y registra el método callback que se ejecutará cuando el nick haya sido comprobado por el servidor.
        /// </summary>
        public async void SignalR()
        {
            conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            //conn = new HubConnection("https://serverpaintionary.azurewebsites.net");
            //conn = new HubConnection("http://localhost:11111/");
            proxy = conn.CreateHubProxy("PictionaryHub");
            Connection.Connection.conn = conn;
            Connection.Connection.proxy = proxy;

            await conn.Start();

            proxy.On<bool>("nickComprobado", OnNickComprobado);
        }

        /// <summary>
        /// Se ejecutará cuando el servidor haya comprobado el nick.
        /// 
        /// Si es válido, navegará a la pantalla del listado de salas, si no, mostrará un mensaje de error.
        /// </summary>
        /// <param name="isNickUnico">true si es válido o false si no es válido</param>
        private async void OnNickComprobado(bool isNickUnico)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (isNickUnico)
                {
                    //Debe viajar hacia delante
                    visibilidadMensajeError = "Collapsed";
                    NotifyPropertyChanged("visibilidadMensajeError");
                    navigationFrame.Navigate(typeof(ListadoSalas), nick);
                }
                else
                {
                    //Debe mostrar mensaje de error
                    visibilidadMensajeError = "Visible";
                    NotifyPropertyChanged("visibilidadMensajeError");
                }
            });
               
        }

        public String nick {
            get { return _nick; }
            set {
                if (_nick != value)
                {
                    _nick = value;
                    _entrarAlJuegoCommand.RaiseCanExecuteChanged();
                    NotifyPropertyChanged("nick");
                }
            }
        }

        public DelegateCommand EntrarCommand
        {
            get
            {
                _entrarAlJuegoCommand = new DelegateCommand(EntrarAlJuego_Executed, EntrarAlJuego_CanExecuted);
                return _entrarAlJuegoCommand;
            }
        }

        /// <summary>
        /// Se realiza cuando se ejecuta el comando de entrar al juego.
        /// 
        /// Se pide al servidor que compruebe el nick introducido en el campo.
        /// </summary>
        public void EntrarAlJuego_Executed()
        {
            //Aquí deberá hacer un invoke al servidor 
            proxy.Invoke("comprobarNickUnico", _nick);
            //navigationFrame.Navigate(typeof(ListadoSalas),nick);
        }

        /// <summary>
        /// Indica si el comando de entrar al juego está habilitado o no.
        /// 
        /// Estará habilitado cuando haya algo escrito en el campo del nickname
        /// </summary>
        /// <returns></returns>
        public bool EntrarAlJuego_CanExecuted()
        {
            bool puedeEntrar = false;

            if (!String.IsNullOrEmpty(_nick)) {
                puedeEntrar = true;
            }

            return puedeEntrar;
        }
    }
}
