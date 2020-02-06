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

        public ObservableCollection<clsPartida> partidasAMostrar
        {
            get

            {
                ObservableCollection<clsPartida> partidas = new ObservableCollection<clsPartida>();
                if (ListadoPartidas != null)
                partidas = new ObservableCollection<clsPartida>(from p in ListadoPartidas
                       where p.ListadoJugadores.Count < p.NumeroMaximoJugadores
                       select p);

                return partidas;
                       
            } 
        }
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
            //conn = new HubConnection("http://localhost:11111/");
            proxy = conn.CreateHubProxy("PictionaryHub");
            Connection.Connection.conn = conn;
            Connection.Connection.proxy = proxy;

            await conn.Start();


            proxy.On<List<clsPartida>>("recibirSalas",pedirListaAsync);
            proxy.On<string>("eliminarPartidaVacia", eliminarPartidaVacia);
            proxy.On<clsJugador, clsPartida>("jugadorAdded", jugadorAdded);
            proxy.On<string, string>("jugadorDeletedSala", jugadorDeleted);
            proxy.Invoke("sendSalas");

            
        }

        private async void eliminarPartidaVacia(string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clsPartida partida = ListadoPartidas.First<clsPartida>(x => x.NombreSala == nombreSala);
                if (partida != null)
                {
                    ListadoPartidas.Remove(partida);
                    NotifyPropertyChanged("partidasAMostrar");
                }
            });
        }

        public async void jugadorDeleted(string usuario, string nombreSala)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                clsPartida partida;
                try
                {
                    partida = ListadoPartidas.First<clsPartida>(x => x.NombreSala == nombreSala);
                }catch (Exception e) {
                    partida = null;
                }
                if (partida != null)
                {
                    clsJugador jugador;
                    try
                    {
                        jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuario);
                    }catch (Exception e) {
                        jugador = null;
                    }
                    if (jugador != null)
                    {
                        partida.ListadoJugadores.Remove(jugador);
                        partida.NotifyPropertyChanged("ListadoPartidas");
                        NotifyPropertyChanged("partidasAMostrar");
                    }
                }
            });
        }

        private async void pedirListaAsync(List<clsPartida> listado)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                this._listadoPartidas = new ObservableCollection<clsPartida>(listado);
                NotifyPropertyChanged("ListadoPartidas");
                NotifyPropertyChanged("partidasAMostrar");
            }

            );
        }



        public void ListadoSalas_Tapped(clsPartida partida)
        {

            proxy.Invoke("addJugadorToSala", partida.NombreSala, _usuarioPropio);
            navigationService.NavigateTo(ViewModelLocator.SalaEspera,_usuarioPropio.Nickname);
        }





        public async void jugadorAdded( clsJugador jugador, clsPartida game)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                clsPartida partida;
                try {
                    partida = this.ListadoPartidas.First<clsPartida>(x => x.NombreSala == game.NombreSala);
                } catch (Exception e) {
                    partida = null;
                }

                if(partida != null)
                {
                    partida.ListadoJugadores.Add(jugador);
                    NotifyPropertyChanged("ListadoPartidas");
                    NotifyPropertyChanged("partidasAMostrar");

                }
            });
        }


        #endregion

    }
}
