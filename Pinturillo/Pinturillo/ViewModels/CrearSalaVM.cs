using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

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


        public CrearSalaVM(INavigationService navigationService)
        {
            _partida = new clsPartida();
            this.navigationService = navigationService;

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
            conn = new HubConnection("http://localhost:11111/");
            proxy = conn.CreateHubProxy("PictionaryHub");
            await conn.Start();


            proxy.On<clsPartida>("salaCreada", salaCreada);
            //proxy.Invoke("sendSalas");
        }

        private async void salaCreada(clsPartida partida)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.navigationService.NavigateTo(ViewModelLocator.SalaEspera, NombreUsuario);
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

        private void CrearCommand_Executed()
        {
            //this.navigationService.NavigateTo(ViewModelLocator.SalaEspera, NombreUsuario);   
            proxy.Invoke("añadirPartida", _partida, _nombreUsuario);
            //var hola = "hola";
        }


        private bool CrearCommand_CanExecuted()
        {
            return true;
        }
    }
}
