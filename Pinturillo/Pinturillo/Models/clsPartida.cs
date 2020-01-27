using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinturillo.Models
{
    public class clsPartida
    {
        #region"Atributos"
        private bool _isJugandose;
        private clsMensaje _mensaje;
        private ObservableCollection<clsMensaje> _listadoMensajes;
        //TODO falta poner el canvas, o los stroke, o lo que sea
        private string _nombreSala;
        private string _password; //-> si es null / vacia, es privada la partida
        private ObservableCollection<clsJugador> _listadoJugadores;
        private string _palabraEnJuego;
        private int _numeroRondasGlobales;
        private int _turno;
        private int _numeroMaximoJugadores;
        //private bool _isPrivada;
        private int _rondaActual;
        private string _connectionIDJugadorActual;

        #endregion

        #region"Constructor"
        public clsPartida(bool isJugandose, clsMensaje mensaje, ObservableCollection<clsMensaje> listadoMensajes, string nombreSala, string password, ObservableCollection<clsJugador> listadoJugadores, string palabraEnJuego, int numeroRondasGlobales, int turno, int numeroMaximoJugadores, int rondaActual, string connectionIDJugadorActual)
        {
            IsJugandose = isJugandose;
            Mensaje = mensaje;
            ListadoMensajes = listadoMensajes;
            NombreSala = nombreSala;
            Password = password;
            ListadoJugadores = listadoJugadores;
            PalabraEnJuego = palabraEnJuego;
            NumeroRondasGlobales = numeroRondasGlobales;
            Turno = turno;
            NumeroMaximoJugadores = numeroMaximoJugadores;
//            _isPrivada = isPrivada;
            RondaActual = rondaActual;
            ConnectionIDJugadorActual = connectionIDJugadorActual;
        }
        #endregion

        #region"Propiedades públicas"
        public bool IsJugandose { get => _isJugandose; set => _isJugandose = value; }
        public clsMensaje Mensaje { get => _mensaje; set => _mensaje = value; }
        public ObservableCollection<clsMensaje> ListadoMensajes { get => _listadoMensajes; set => _listadoMensajes = value; }
        public string NombreSala { get => _nombreSala; set => _nombreSala = value; }
        public string Password { get => _password; set => _password = value; }
        public ObservableCollection<clsJugador> ListadoJugadores { get => _listadoJugadores; set => _listadoJugadores = value; }
        public string PalabraEnJuego { get => _palabraEnJuego; set => _palabraEnJuego = value; }
        public int NumeroRondasGlobales { get => _numeroRondasGlobales; set => _numeroRondasGlobales = value; }
        public int Turno { get => _turno; set => _turno = value; }
        public int NumeroMaximoJugadores { get => _numeroMaximoJugadores; set => _numeroMaximoJugadores = value; }
        public int RondaActual { get => _rondaActual; set => _rondaActual = value; }
        public string ConnectionIDJugadorActual { get => _connectionIDJugadorActual; set => _connectionIDJugadorActual = value; }
        #endregion
    }
}
