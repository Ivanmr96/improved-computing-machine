using Pictionary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pictionary.ViewModel
{
    class SalaEsperaVM
    {
        #region atributos privados

        private clsPartida partida;
        private clsMensaje mensaje;

        #endregion

        #region constructor

        public SalaEsperaVM()
        {
            // Aquí obtendría la partida enviada desde la otra ventana

            partida = new clsPartida();

            partida.ListadoJugadores.Add(new clsJugador("id", 0, "Ivan", false, false, false));
            partida.ListadoJugadores.Add(new clsJugador("id", 0, "Pepe", false, false, false));
        }

        #endregion

        #region propiedades publicas

        public clsPartida Partida
        {
            get => partida;
            set => partida = value;
        }
        public clsMensaje Mensaje
        {
            get => mensaje;
            set => mensaje = value;
        }

        #endregion

        #region commands

        public Comando enviarMensaje { get; set; }

        private bool enviarMensaje_canExecute() => mensaje != null || mensaje.Mensaje != "";

        private void enviarMensaje_execute()
        {
            //Mandar el mensaje al servidor
        }

        public Comando salir { get; set; }

        private void salir_execute()
        {
            //Indica al serivdor que sale.
        }

        public Comando comenzarPartida { get; set; }

        public bool comenzarPartida_canExecute()
        {
            //Puede ejecutar si es el lider
            return true;
        }

        public void comenzarPartida_execute()
        {
            //Indica al servidro que la partida va a comenzar.
        }

        #endregion
    }
}
