using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinturillo.ViewModels
{
    public class CrearSalaVM : clsVMBase
    {
        private String _nombreUsuario;
        //private clsPartida _partida;
        private String _lblErrorNombreSala; //Si la sala ya existe muestra este label
        private String _lblErrorContrasena; //Si se marca el checkbox de sala privada y no se escribe contrasena
        private DelegateCommand _crearPartida;

        public CrearSalaVM()
        {
            //_partida = new clsPartida();
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

        private async void CrearCommand_Executed()
        {
            
        }


        private bool CrearCommand_CanExecuted()
        {
            bool creable = false;

            //Si la sala no tiene nombre no se puede crear

            return creable;
        }
    }
}
