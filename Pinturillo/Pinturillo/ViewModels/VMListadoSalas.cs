using Microsoft.AspNet.SignalR.Client;
using Pinturillo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinturillo.ViewModels
{
    public class VMListadoSalas : clsVMBase
    {
        #region"Atributos privados"
        private ObservableCollection<clsPartida> _listadoPartidas;
        private clsJugador _usuarioPropio;
        private HubConnection conn;
        private IHubProxy proxy;

        #endregion

        #region"Propiedades públicas"
        public DelegateCommand JoinGroupCommand { get; }
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
        #endregion

        #region"Constructor"
        public VMListadoSalas()
        {
            this._usuarioPropio = new clsJugador();

            //TODO todas las cosas de SignalR


            //Command
            this.JoinGroupCommand = new DelegateCommand(ExecuteJoinGroupCommand, CanExecuteJoinGroupCommand);

        }
        #endregion

    }
}
