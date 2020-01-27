using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Pinturillo.ViewModels
{
    public class VMPantallaJuego : clsVMBase
    {
        #region"Atributos privados"
        private ObservableCollection<clsMensaje> _listadoMensajesChat;
        private clsJugador _usuarioPropio;
        private clsMensaje _mensaje;
        private int _timeMax;
        private DispatcherTimer _dispatcherTimer;
        private clsPartida _partida;
        private string _lblTemporizador;
        private HubConnection conn;
        private IHubProxy proxy;

        #endregion

        #region"Propiedades públicas"
        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand SendMessageCommand { get; }
        public DispatcherTimer DispatcherTimer { get => _dispatcherTimer; set => _dispatcherTimer = value; }
        public clsPartida Partida { get => _partida; set => _partida = value; }
        public string LblTemporizador { get => _lblTemporizador; set => _lblTemporizador = value; }
        public ObservableCollection<clsMensaje> ListadoMensajesChat { get => _listadoMensajesChat; set => _listadoMensajesChat = value; }
        public clsJugador UsuarioPropio { get => _usuarioPropio; set => _usuarioPropio = value; }
        public clsMensaje Mensaje { get => _mensaje; set => _mensaje = value; }

        #endregion


        #region"Command"
        private void ExecuteGoBackCommand()
        {
            //TODO
        }

        private bool CanExecuteGoBackCommand()
        {
            //TODO
            return false;
        }

        private void ExecuteSendMessageCommand()
        {
            //TODO
        }

        private bool CanExecuteSendMessageCommand()
        {
            //TODO
            return false;
        }
        #endregion
    }
}
