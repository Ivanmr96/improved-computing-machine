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

        public void sendSalas()
        {
            //List<clsPartida> listadoSalas = new List<clsPartida>();
            //listadoSalas.Add(new clsPartida(false, null, null, "Sala1", null, null, "Palabra1", 5, 0, 4, 0, null));
            //for (int i = 0; i < 50; i++)
            //{
            //    listadoSalas.Add(new clsPartida(false, null, null, "Sala " + i, null, null, "Palabra1", 5, 0, 4, 0, null));
            //}

            Clients.Caller.recibirSalas(listadoSalas.ListadoPartidas);
        }

        public void añadirPartida(clsPartida partida, String nickLider)
        {
            //clsJugador jugador = partida.ListadoJugadores.First<clsJugador>(j => j.ConnectionID == Context.ConnectionId);

            clsJugador lider = new clsJugador(Context.ConnectionId, 0, nickLider, false, false, true);

            partida.ListadoJugadores.Add(lider);

            listadoSalas.ListadoPartidas.Add(partida);

            Clients.All.recibirSalas(listadoSalas.ListadoPartidas);
            Clients.Caller.salaCreada(partida);
        }



        public void sendMensaje (clsMensaje mensaje, string nombreGrupo)
        {

            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);

            if(partida != null)
            {
                partida.ListadoMensajes.Add(mensaje);
                //Clients.Group(nombreGrupo).addMensajeToChat(mensaje);
                Clients.All.addMensajeToChat(mensaje);
            }

            
        }



        public void addJugadorToSala(string nombreGrupo, clsJugador jugador)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreGrupo);
            if(partida != null)
            {

                if (partida.ListadoJugadores.Count < partida.NumeroMaximoJugadores)
                {
                    partida.ListadoJugadores.Add(jugador);
                    Clients.All.jugadorAdded(jugador, nombreGrupo);
                }
                   
            }
            
        }

        private clsPartida obtenerPartidaPorNombreSala(String nombreGrupo)
        {
            return listadoSalas.ListadoPartidas.Find(x => x.NombreSala == nombreGrupo);
        }

        public void jugadorHaSalido(string usuario, string nombreSala)
        {
            clsPartida partida = obtenerPartidaPorNombreSala(nombreSala);

            clsJugador jugador = partida.ListadoJugadores.First<clsJugador>(j => j.Nickname == usuario);

            if(jugador != null)
            {
                partida.ListadoJugadores.Remove(jugador);

                if(partida.ListadoJugadores.Count == 0)
                {
                    listadoSalas.ListadoPartidas.Remove(partida);
                    Clients.All.eliminarPartidaVacia(partida.NombreSala);
                }
                else
                {
                    Clients.All.jugadorDeletedSala(jugador.Nickname, nombreSala);
                }


                
               
            }

        }

    }
}