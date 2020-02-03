using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Pinturillo.ViewModels
{
    public class VMListadoSalas : clsVMBase
    {
        #region"Atributos privados"
        private ObservableCollection<clsPartida> _listadoPartidas;
        private clsJugador _usuarioPropio;
        private HubConnection conn;
        private IHubProxy proxy;
        private readonly INavigationService navigationService;

        #endregion

        #region"Propiedades públicas"
        public DelegateCommand JoinGroupCommand { get; }
        public DelegateCommand CreateGroupCommand { get; }
        public ObservableCollection<clsPartida> ListadoPartidas { get => _listadoPartidas; set => _listadoPartidas = value; }
        public clsJugador UsuarioPropio { get => _usuarioPropio; set => _usuarioPropio = value; }
        #endregion


        #region"Command"
        private void ExecuteJoinGroupCommand()
        {
            //TODO
        }

        private bool CanExecuteJoinGroupCommand()
        {
            //TODO
            return false;
        }

        private void ExecuteCreateGroupCommand()
        {
            navigationService.NavigateTo(ViewModelLocator.CrearSala, this._usuarioPropio.Nickname);
        }
        #endregion

        #region"Constructor"
        public VMListadoSalas(INavigationService navigationService)
        {
            this._usuarioPropio = new clsJugador();
            this.navigationService = navigationService;
            //TODO todas las cosas de SignalR
            //Command
            this.JoinGroupCommand = new DelegateCommand(ExecuteJoinGroupCommand, CanExecuteJoinGroupCommand); //TODO borrar command
            this.CreateGroupCommand = new DelegateCommand(ExecuteCreateGroupCommand);

            SignalR();
            

        }

        public async void SignalR()
        {
            conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            proxy = conn.CreateHubProxy("PictionaryHub");
            await conn.Start();


            proxy.On<List<clsPartida>>("recibirSalas",pedirListaAsync);
            proxy.On<clsJugador, string>("jugadorAdded", jugadorAdded);
            proxy.Invoke("sendSalas");
        }

        private async void pedirListaAsync(List<clsPartida> listado)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                
                this._listadoPartidas = new ObservableCollection<clsPartida>(listado);
                NotifyPropertyChanged("ListadoPartidas");

            }

            );
        }



        public void ListadoSalas_Tapped(clsPartida partida)
        {
            proxy.Invoke("addJugadorToSala", partida.NombreSala, _usuarioPropio);
        }


        public async void jugadorAdded( clsJugador jugador, string nombrePartida)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                clsPartida partida = this.ListadoPartidas.First<clsPartida>(x => x.NombreSala == nombrePartida);
                if(partida != null)
                {
                    partida.ListadoJugadores.Add(jugador);
                    NotifyPropertyChanged("ListadoPartidas");
                }
               

            }

            );

        }


        #endregion

    }
}
