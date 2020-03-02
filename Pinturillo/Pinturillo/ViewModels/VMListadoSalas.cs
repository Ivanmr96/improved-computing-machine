using GalaSoft.MvvmLight.Views;
using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Pinturillo.ViewModels
{
    public class VMListadoSalas : clsVMBase
    {
        #region"Atributos privados"
        private ObservableCollection<clsPartida> _listadoPartidas;
        private clsJugador _usuarioPropio;
        private HubConnection conn;
        private IHubProxy proxy;
        Frame navigationFrame = Window.Current.Content as Frame;
        private string contrasena;
        private bool puedesFuncionar;
        //private ICommand enterCommand;

        #endregion

        #region"Propiedades públicas"
        //public DelegateCommand JoinGroupCommand { get; }
        public DelegateCommand EnterCommand { get; }
        public DelegateCommand CreateGroupCommand { get; }
        public ObservableCollection<clsPartida> ListadoPartidas { get => _listadoPartidas; set => _listadoPartidas = value; }
        public clsJugador UsuarioPropio { get => _usuarioPropio; set => _usuarioPropio = value; }
        public clsPartida partidaSeleccionada { get; set; }
        public bool DialogContrasenaVisibility { get; set; }

        public string Contrasena
        {
            get { return contrasena; }
            set 
            { 
                contrasena = value;
                EnterCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ContrasenaIncorrecta 
        { 
            get; 
            set; 
        }

        public ObservableCollection<clsPartida> partidasAMostrar
        {
            get

            {
                ObservableCollection<clsPartida> partidas = new ObservableCollection<clsPartida>();
                if (ListadoPartidas != null)
                    partidas = new ObservableCollection<clsPartida>(from p in ListadoPartidas
                                                                    where p.ListadoJugadores.Count < p.NumeroMaximoJugadores
                                                                    && p.IsJugandose == false
                                                                    select p);
                return partidas;
                       
            } 
        }
        #endregion


        #region"Command"
        //private void ExecuteJoinGroupCommand()
        //{
        //    //TODO
        //}

        //private bool CanExecuteJoinGroupCommand()
        //{
        //    //TODO
        //    return false;
        //}

        private void ExecuteCreateGroupCommand()
        {
            if (puedesFuncionar)
            {
                navigationFrame.Navigate(typeof(CrearSalaPage), this._usuarioPropio.Nickname);
                puedesFuncionar = false;
            }
           
        }

        private void ExecuteEnterContrasenaCommand()
        {
            //TODO Comprobar que la contraseña es correcta, si lo es, entrar en la sala, si no, mostrar un texto diciendo que es incorrecta
            
            if(contrasena == partidaSeleccionada.Password)
            {
                ContrasenaIncorrecta = false;

                proxy.Invoke("addJugadorToSala", partidaSeleccionada.NombreSala, _usuarioPropio);
                navigationFrame.Navigate(typeof(SalaEspera), this._usuarioPropio.Nickname);

                DialogContrasenaVisibility = false;

                NotifyPropertyChanged("DialogContrasenaVisibility");
            }
            else
                ContrasenaIncorrecta = true;



            NotifyPropertyChanged("ContrasenaIncorrecta");
        }

        private bool CanExecuteEnterContrasenaCommand()
        {
            return !String.IsNullOrEmpty(contrasena);
        }

        #endregion

        #region"Constructor"
        public VMListadoSalas()
        {
            puedesFuncionar = true;
            this._usuarioPropio = new clsJugador();
            //TODO todas las cosas de SignalR
            //Command
            /*this.JoinGroupCommand = new DelegateCommand(ExecuteJoinGroupCommand, CanExecuteJoinGroupCommand);*/ //TODO borrar command
            this.CreateGroupCommand = new DelegateCommand(ExecuteCreateGroupCommand);
            this.EnterCommand = new DelegateCommand(ExecuteEnterContrasenaCommand, CanExecuteEnterContrasenaCommand);
            this.puedesFuncionar = true;
            SignalR();
            
        
        }

        public async void SignalR()
        {
            //conn = new HubConnection("https://pictionary-di.azurewebsites.net");
            //conn = new HubConnection("http://localhost:11111/");
            //proxy = conn.CreateHubProxy("PictionaryHub");
            conn = Connection.Connection.conn;
            proxy = Connection.Connection.proxy;

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
                clsPartida partida;
                try
                {
                    partida = ListadoPartidas.First<clsPartida>(x => x.NombreSala == nombreSala);
                }
                catch (Exception e)
                {
                    partida = null;
                }
                    
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

                if(ListadoPartidas != null)
                {
                    partida = ListadoPartidas.FirstOrDefault<clsPartida>(x => x.NombreSala == nombreSala);

                    if (partida != null)
                    {
                        clsJugador jugador;

                        jugador = partida.ListadoJugadores.FirstOrDefault<clsJugador>(j => j.Nickname == usuario);

                        if (jugador != null)
                        {
                            partida.ListadoJugadores.Remove(jugador);
                            partida.NotifyPropertyChanged("ListadoPartidas");
                            NotifyPropertyChanged("partidasAMostrar");
                        }
                    }
                }
            });
        }

        private async void pedirListaAsync(List<clsPartida> listado)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (puedesFuncionar)
                {                
                    this._listadoPartidas = new ObservableCollection<clsPartida>(listado);
                    NotifyPropertyChanged("ListadoPartidas");
                    NotifyPropertyChanged("partidasAMostrar");
                }               
            });
        }



        public void ListadoSalas_Tapped(clsPartida partida)
        {
            partidaSeleccionada = partida;
            if (!String.IsNullOrEmpty(partidaSeleccionada.Password)) { 
                DialogContrasenaVisibility = true;
                NotifyPropertyChanged("DialogContrasenaVisibility");
            }else
            {
                proxy.Invoke("addJugadorToSala", partida.NombreSala, _usuarioPropio);

                //Aqui necesito enviarle dos parametros, el usuario propio nickname y el nombre de la sala seleccionada
                Tuple<string, string> nickYNombrePartida = 
                    new Tuple<string, string>(this._usuarioPropio.Nickname, partida.NombreSala);

                // navigationFrame.Navigate(typeof(SalaEspera), this._usuarioPropio.Nickname);
                navigationFrame.Navigate(typeof(SalaEspera), nickYNombrePartida);
            }
            puedesFuncionar = true;

        }

        public async void jugadorAdded( clsJugador jugador, clsPartida game)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               
                    clsPartida partida;

                    partida = this.ListadoPartidas.FirstOrDefault<clsPartida>(x => x.NombreSala == game.NombreSala);


                    if (partida != null)
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
