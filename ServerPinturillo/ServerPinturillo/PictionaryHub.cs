using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using ServerPinturillo.Clases;

namespace ServerPinturillo
{
    public class PictionaryHub : Hub
    {

        //Listado de salas que hay en el juego
        private SingletonSalas listadoSalas;

        public PictionaryHub(SingletonSalas salas)
        {
            this.listadoSalas = salas;
        }

        /*
         -crear partida
         -enviar listado de salas
         -enviar lista de mensajes 
         -enviar el mensaje añadido
         -notificar que se ha unido una persona a la partida 
         -notificar que la partida ha empezado para que comiencen a contar el timer
         -enviar partida (solo se enviaría al usuario que crea la partido)
         -
         */



    }
}